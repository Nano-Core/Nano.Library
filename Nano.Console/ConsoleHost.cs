//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Runtime.ExceptionServices;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Hosting.Server;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Nano.Console;
//using Nano.Console.Hosting;
//using Nano.Console.Hosting.Interfaces;

//namespace ConsoleHost
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    internal class ConsoleHost : IConsoleHost
//    {
//        private readonly IServiceCollection _applicationServiceCollection;
//        private IStartup _startup;
//        private ApplicationLifetime _applicationLifetime;
//        private HostedServiceExecutor _hostedServiceExecutor;

//        private readonly IServiceProvider _hostingServiceProvider;
//        private readonly ConsoleHostOptions _options;
//        private readonly IConfiguration _config;
//        private readonly AggregateException _hostingStartupErrors;

//        private ExceptionDispatchInfo _applicationServicesException;
//        private ILogger<ConsoleHost> _logger;

//        private bool _stopped;

//        private IServer Server { get; }

//        public ConsoleHost(IServiceCollection appServices, IServiceProvider hostingServiceProvider, ConsoleHostOptions options, IConfiguration config, AggregateException hostingStartupErrors)
//        {
//            if (appServices == null)
//                throw new ArgumentNullException(nameof(appServices));

//            if (hostingServiceProvider == null)
//                throw new ArgumentNullException(nameof(hostingServiceProvider));

//            if (config == null)
//                throw new ArgumentNullException(nameof(config));

//            this._config = config;
//            this._hostingStartupErrors = hostingStartupErrors;
//            this._options = options;
//            this._applicationServiceCollection = appServices;
//            this._hostingServiceProvider = hostingServiceProvider;
//            this._applicationServiceCollection.AddSingleton<IApplicationLifetime, ApplicationLifetime>();
//            this._applicationServiceCollection.AddSingleton(x => x.GetRequiredService<IApplicationLifetime>() as Microsoft.Extensions.Hosting.IApplicationLifetime);
//            this._applicationServiceCollection.AddSingleton<HostedServiceExecutor>();

//            this.Server = new ConsoleServer();
//        }

//        public IServiceProvider Services { get; private set; }

//        public void Initialize()
//        {
//            try
//            {
//                this.EnsureApplicationServices();
//            }
//            catch (Exception ex)
//            {
//                if (Services == null)
//                {
//                    Services = _applicationServiceCollection.BuildServiceProvider();
//                }

//                if (!_options.CaptureStartupErrors)
//                {
//                    throw;
//                }

//                _applicationServicesException = ExceptionDispatchInfo.Capture(ex);
//            }
//        }

//        public void Start()
//        {
//            StartAsync().GetAwaiter().GetResult();
//        }

//        public virtual async Task StartAsync(CancellationToken cancellationToken = default)
//        {
//            HostingEventSource.Log.HostStart();
//            _logger = Services.GetRequiredService<ILogger<ConsoleHost>>();

//            var application = BuildApplication();

//            _applicationLifetime = Services.GetRequiredService<IApplicationLifetime>() as ApplicationLifetime;
//            _hostedServiceExecutor = Services.GetRequiredService<HostedServiceExecutor>();
//            var diagnosticSource = Services.GetRequiredService<DiagnosticListener>();
//            var httpContextFactory = Services.GetRequiredService<IHttpContextFactory>();
//            var hostingApp = new HostingApplication(application, _logger, diagnosticSource, httpContextFactory);
//            await Server.StartAsync(hostingApp, cancellationToken).ConfigureAwait(false);

//            // Fire IApplicationLifetime.Started
//            _applicationLifetime?.NotifyStarted();

//            // Fire IHostedService.Start
//            await _hostedServiceExecutor.StartAsync(cancellationToken).ConfigureAwait(false);

//            // Log the fact that we did load hosting startup assemblies.
//            if (_logger.IsEnabled(LogLevel.Debug))
//            {
//                foreach (var assembly in _options.HostingStartupAssembliesFinal)
//                {
//                    _logger.LogDebug("Loaded hosting startup assembly {assemblyName}", assembly);
//                }
//            }

//            if (_hostingStartupErrors != null)
//            {
//                foreach (var exception in _hostingStartupErrors.InnerExceptions)
//                {
//                    _logger.LogError(exception.Message);
//                }
//            }
//        }

//        private void EnsureApplicationServices()
//        {
//            if (Services == null)
//            {
//                EnsureStartup();
//                Services = _startup.ConfigureServices(_applicationServiceCollection);
//            }
//        }
//        private void EnsureStartup()
//        {
//            if (_startup != null)
//            {
//                return;
//            }

//            _startup = _hostingServiceProvider.GetService<IStartup>();

//            if (_startup == null)
//            {
//                throw new InvalidOperationException($"No startup configured. Please specify startup via ConsoleHostBuilder.UseStartup, ConsoleHostBuilder.Configure, injecting {nameof(IStartup)} or specifying the startup assembly via {nameof(ConsoleHostDefaults.startupAssemblyKey)} in the console host configuration.");
//            }
//        }
//        private RequestDelegate BuildApplication()
//        {
//            try
//            {
//                _applicationServicesException?.Throw();

//                var builderFactory = Services.GetRequiredService<IApplicationBuilderFactory>();
//                var builder = builderFactory.CreateBuilder(Server.Features);
//                builder.ApplicationServices = Services;

//                var startupFilters = Services.GetService<IEnumerable<IStartupFilter>>();
//                Action<IApplicationBuilder> configure = _startup.Configure;
//                foreach (var filter in startupFilters.Reverse())
//                {
//                    configure = filter.Configure(configure);
//                }

//                configure(builder);

//                return builder.Build();
//            }
//            catch (Exception ex)
//            {
//                if (!_options.SuppressStatusMessages)
//                {
//                    Console.WriteLine("Application startup exception: " + ex.Message);
//                }
//                var logger = Services.GetRequiredService<ILogger<ConsoleHost>>();
//                logger.LogError(ex.Message);

//                if (!_options.CaptureStartupErrors)
//                {
//                    throw;
//                }

//                return null;
//            }
//        }

//        public async Task StopAsync(CancellationToken cancellationToken = default)
//        {
//            if (_stopped)
//            {
//                return;
//            }
//            _stopped = true;

//            var timeoutToken = new CancellationTokenSource(_options.ShutdownTimeout).Token;
//            cancellationToken = !cancellationToken.CanBeCanceled 
//                ? timeoutToken 
//                : CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutToken).Token;

//            _applicationLifetime?.StopApplication();

//            if (Server != null)
//            {
//                await Server.StopAsync(cancellationToken).ConfigureAwait(false);
//            }

//            if (_hostedServiceExecutor != null)
//            {
//                await _hostedServiceExecutor.StopAsync(cancellationToken).ConfigureAwait(false);
//            }

//            _applicationLifetime?.NotifyStopped();

//            HostingEventSource.Log.HostStop();
//        }

//        public void Dispose()
//        {
//            if (!_stopped)
//            {
//                try
//                {
//                    StopAsync().GetAwaiter().GetResult();
//                }
//                catch (Exception ex)
//                {
//                    _logger?.LogError(ex.Message);
//                }
//            }

//            (Services as IDisposable)?.Dispose();
//            (_hostingServiceProvider as IDisposable)?.Dispose();
//        }
//    }
//}