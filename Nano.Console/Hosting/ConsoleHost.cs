using System;
using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nano.Console.Hosting.Interfaces;

namespace Nano.Console.Hosting
{
    /// <summary>
    /// Console Host.
    /// </summary>
    internal class ConsoleHost : IConsoleHost
    {
        protected bool stopped;
        protected ILogger logger;
        protected IStartup startup;
        protected ApplicationLifetime applicationLifetime;
        protected ExceptionDispatchInfo applicationServicesException;

        protected readonly IConfiguration configuration;
        protected readonly IServiceProvider hostingServiceProvider;
        protected readonly IServiceCollection applicationServiceCollection;
        protected readonly ConsoleHostOptions options;
        protected readonly AggregateException hostingStartupErrors;

        /// <inheritdoc />
        public virtual IServiceProvider Services { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="appServices">The <see cref="IServiceCollection"/>.</param>
        /// <param name="hostingServiceProvider">The <see cref="IServiceProvider"/>.</param>
        /// <param name="options">The <see cref="ConsoleHostOptions"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <param name="hostingStartupErrors">The <see cref="AggregateException"/>.</param>
        public ConsoleHost(IServiceCollection appServices, IServiceProvider hostingServiceProvider, ConsoleHostOptions options, IConfiguration configuration, AggregateException hostingStartupErrors)
        {
            if (appServices == null)
                throw new ArgumentNullException(nameof(appServices));

            if (hostingServiceProvider == null)
                throw new ArgumentNullException(nameof(hostingServiceProvider));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            this.hostingServiceProvider = hostingServiceProvider;
            this.options = options;
            this.configuration = configuration;
            this.hostingStartupErrors = hostingStartupErrors;

            this.applicationServiceCollection = appServices;
            this.applicationServiceCollection.AddSingleton<IApplicationLifetime, ApplicationLifetime>();
            this.applicationServiceCollection.AddSingleton(x => x.GetRequiredService<IApplicationLifetime>() as Microsoft.Extensions.Hosting.IApplicationLifetime);
        }

        /// <inheritdoc />
        public virtual void Start()
        {
            this.logger = this.Services.GetRequiredService<ILogger<ConsoleHost>>();

            this.applicationLifetime = Services.GetRequiredService<IApplicationLifetime>() as ApplicationLifetime;
            this.applicationLifetime?.NotifyStarted();

            if (this.logger.IsEnabled(LogLevel.Debug))
            {
                foreach (var assembly in this.options.HostingStartupAssembliesFinal)
                {
                    this.logger.LogDebug("Loaded hosting startup assembly {assemblyName}", assembly);
                }
            }

            if (this.hostingStartupErrors != null)
            {
                foreach (var exception in this.hostingStartupErrors.InnerExceptions)
                {
                    this.logger.LogError(exception.Message);
                }
            }
        }

        /// <inheritdoc />
        public virtual void Stop()
        {
            if (this.stopped)
                return;

            this.stopped = true;

            this.applicationLifetime?.StopApplication();
            this.applicationLifetime?.NotifyStopped();
        }

        /// <inheritdoc />
        public virtual void Initialize()
        {
            try
            {
                this.EnsureApplicationServices();
            }
            catch (Exception ex)
            {
                if (this.Services == null)
                {
                    this.Services = this.applicationServiceCollection.BuildServiceProvider();
                }

                if (!this.options.CaptureStartupErrors)
                {
                    throw;
                }

                this.applicationServicesException = ExceptionDispatchInfo.Capture(ex);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!this.stopped)
            {
                try
                {
                    this.Stop();
                }
                catch (Exception ex)
                {
                    this.logger?.LogError(ex.Message);
                }
            }

            (this.Services as IDisposable)?.Dispose();
            (this.hostingServiceProvider as IDisposable)?.Dispose();
        }

        private void EnsureStartup()
        {
            if (this.startup != null)
                return;

            this.startup = this.hostingServiceProvider.GetService<IStartup>();

            if (this.startup == null)
                throw new InvalidOperationException($"No startup configured. Please specify startup via ConsoleHostBuilder.UseStartup, ConsoleHostBuilder.Configure, injecting {nameof(IStartup)} or specifying the startup assembly via {nameof(ConsoleHostDefaults.StartupAssemblyKey)} in the console host configuration.");
        }
        private void EnsureApplicationServices()
        {
            if (this.Services == null)
            {
                this.EnsureStartup();

                this.Services = this.startup.ConfigureServices(this.applicationServiceCollection);
            }
        }
    }
}