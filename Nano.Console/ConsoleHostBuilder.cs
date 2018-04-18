//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.ExceptionServices;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection.Extensions;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.ObjectPool;
//using Nano.Console;
//using Nano.Console.Hosting;
//using Nano.Console.Hosting.Interfaces;

//namespace ConsoleHost
//{
//    // TODO: https://stackoverflow.com/questions/41407221/startup-cs-in-a-self-hosted-net-core-console-application


//    /// <summary>
//    /// A builder for <see cref="IConsoleHost"/>
//    /// </summary>
//    public class ConsoleHostBuilder : IConsoleHostBuilder
//    {
//        private bool consoleHostBuilt;
//        private readonly IConfiguration configuration;

//        private ConsoleHostOptions options;
//        private readonly ConsoleHostBuilderContext context;
//        private readonly HostingEnvironment hostingEnvironment = new HostingEnvironment();
//        private readonly List<Action<ConsoleHostBuilderContext, IServiceCollection>> configureServicesDelegates = new List<Action<ConsoleHostBuilderContext, IServiceCollection>>();
//        private readonly List<Action<ConsoleHostBuilderContext, IConfigurationBuilder>> configureAppConfigurationBuilderDelegates = new List<Action<ConsoleHostBuilderContext, IConfigurationBuilder>>();

//        /// <summary>
//        /// Initializes a new instance of the <see cref="ConsoleHostBuilder"/> class.
//        /// </summary>
//        public ConsoleHostBuilder()
//        {
//            this.configuration = new ConfigurationBuilder()
//                .AddEnvironmentVariables("ASPNETCORE_")
//                .Build();

//            this.context = new ConsoleHostBuilderContext
//            {
//                Configuration = this.configuration
//            };

//            if (string.IsNullOrEmpty(this.GetSetting(ConsoleHostDefaults.environmentKey)))
//            {
//                var value = Environment.GetEnvironmentVariable("Hosting:Environment") ?? Environment.GetEnvironmentVariable("ASPNET_ENV");
//                this.UseSetting(ConsoleHostDefaults.environmentKey, value);
//            }
//        }

//        /// <summary>
//        /// Get the setting value from the configuration.
//        /// </summary>
//        /// <param name="key">The key of the setting to look up.</param>
//        /// <returns>The value the setting currently contains.</returns>
//        public virtual string GetSetting(string key)
//        {
//            if (key == null)
//                throw new ArgumentNullException(nameof(key));

//            return this.configuration[key];
//        }

//        /// <summary>
//        /// Add or replace a setting in the configuration.
//        /// </summary>
//        /// <param name="key">The key of the setting to add or replace.</param>
//        /// <param name="value">The value of the setting to add or replace.</param>
//        /// <returns>The <see cref="IConsoleHostBuilder"/>.</returns>
//        public virtual IConsoleHostBuilder UseSetting(string key, string value)
//        {
//            if (key == null)
//                throw new ArgumentNullException(nameof(key));

//            this.configuration[key] = value;

//            return this;
//        }

//        /// <summary>
//        /// Adds a delegate for configuring additional services for the host or console application. 
//        /// This may be called multiple times.
//        /// </summary>
//        /// <param name="configureServices">A delegate for configuring the <see cref="IServiceCollection"/>.</param>
//        /// <returns>The <see cref="IConsoleHostBuilder"/>.</returns>
//        public virtual IConsoleHostBuilder ConfigureServices(Action<IServiceCollection> configureServices)
//        {
//            if (configureServices == null)
//                throw new ArgumentNullException(nameof(configureServices));

//            return this.ConfigureServices((_, services) => configureServices(services));
//        }

//        /// <summary>
//        /// Adds a delegate for configuring additional services for the host or console application. 
//        /// This may be called multiple times.
//        /// </summary>
//        /// <param name="configureServices">A delegate for configuring the <see cref="IServiceCollection"/>.</param>
//        /// <returns>The <see cref="IConsoleHostBuilder"/>.</returns>
//        public virtual IConsoleHostBuilder ConfigureServices(Action<ConsoleHostBuilderContext, IServiceCollection> configureServices)
//        {
//            if (configureServices == null)
//                throw new ArgumentNullException(nameof(configureServices));

//            this.configureServicesDelegates.Add(configureServices);

//            return this;
//        }

//        /// <summary>
//        /// Adds a delegate for configuring the <see cref="IConfigurationBuilder"/> that will construct an <see cref="IConfiguration"/>.
//        /// </summary>
//        /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder" /> that will be used to construct an <see cref="IConfiguration" />.</param>
//        /// <returns>The <see cref="IConsoleHostBuilder"/>.</returns>
//        /// <remarks>
//        /// The <see cref="IConfiguration"/> and <see cref="ILoggerFactory"/> on the <see cref="ConsoleHostBuilderContext"/> are uninitialized at this stage.
//        /// The <see cref="IConfigurationBuilder"/> is pre-populated with the settings of the <see cref="IConsoleHostBuilder"/>.
//        /// </remarks>
//        public virtual IConsoleHostBuilder ConfigureAppConfiguration(Action<ConsoleHostBuilderContext, IConfigurationBuilder> configureDelegate)
//        {
//            if (configureDelegate == null)
//                throw new ArgumentNullException(nameof(configureDelegate));

//            this.configureAppConfigurationBuilderDelegates.Add(configureDelegate);

//            return this;
//        }

//        /// <summary>
//        /// Builds the required services and an <see cref="IConsoleHost"/> which hosts a console application.
//        /// </summary>
//        public virtual IConsoleHost Build()
//        {
//            if (this.consoleHostBuilt)
//                throw new InvalidOperationException("ConsoleHostBuilder allows creation only of a single instance of ConsoleHost");

//            this.consoleHostBuilt = true;

//            var hostingServices = this.BuildCommonServices(out var hostingStartupErrors);
//            var applicationServices = hostingServices;//todo.Clone();
//            var hostingServiceProvider = GetProviderFromFactory(hostingServices);
//            var logger = hostingServiceProvider.GetRequiredService<ILogger<ConsoleHost>>();

//            if (!this.options.SuppressStatusMessages)
//            {
//                if (Environment.GetEnvironmentVariable("Hosting:Environment") != null)
//                {
//                    logger.LogInformation("The environment variable 'Hosting:Environment' is obsolete and has been replaced with 'ASPNETCORE_ENVIRONMENT'");
//                }

//                if (Environment.GetEnvironmentVariable("ASPNET_ENV") != null)
//                {
//                    logger.LogInformation("The environment variable 'ASPNET_ENV' is obsolete and has been replaced with 'ASPNETCORE_ENVIRONMENT'");
//                }
//            }

//            foreach (var assemblyName in options.HostingStartupAssembliesFinal.GroupBy(a => a, StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1))
//            {
//                logger.LogWarning($"The assembly {assemblyName} was specified multiple times. Hosting startup assemblies should only be specified once.");
//            }

//            this.AddApplicationServices(applicationServices, hostingServiceProvider);
            
//            var host = new ConsoleHost(applicationServices, hostingServiceProvider, options, configuration, hostingStartupErrors);
//            try
//            {
//                host.Initialize();
//                return host;
//            }
//            catch
//            {
//                host.Dispose();
//                throw;
//            }

//            IServiceProvider GetProviderFromFactory(IServiceCollection collection)
//            {
//                var provider = collection.BuildServiceProvider();
//                var factory = provider.GetService<IServiceProviderFactory<IServiceCollection>>();

//                if (factory != null)
//                {
//                    using (provider)
//                    {
//                        return factory.CreateServiceProvider(factory.CreateBuilder(collection));
//                    }
//                }

//                return provider;
//            }
//        }

//        private IServiceCollection BuildCommonServices(out AggregateException hostingStartupErrors)
//        {
//            hostingStartupErrors = null;

//            this.options = new ConsoleHostOptions(this.configuration, Assembly.GetEntryAssembly()?.GetName().Name);

//            //if (!this.options.PreventHostingStartup)
//            //{
//            //    var exceptions = new List<Exception>();

//            //    foreach (var assemblyName in options.HostingStartupAssembliesFinal().Distinct(StringComparer.OrdinalIgnoreCase))
//            //    {
//            //        try
//            //        {
//            //            var assembly = Assembly.Load(new AssemblyName(assemblyName));

//            //            foreach (var attribute in assembly.GetCustomAttributes<HostingStartupAttribute>())
//            //            {
//            //                var hostingStartup = (IHostingStartup)Activator.CreateInstance(attribute.HostingStartupType);
//            //                hostingStartup.Configure(this);
//            //            }
//            //        }
//            //        catch (Exception ex)
//            //        {
//            //            exceptions.Add(new InvalidOperationException($"Startup assembly {assemblyName} failed to execute. See the inner exception for more details.", ex));
//            //        }
//            //    }

//            //    if (exceptions.Count > 0)
//            //    {
//            //        hostingStartupErrors = new AggregateException(exceptions);
//            //    }
//            //}

//            var contentRootPath = this.ResolveContentRootPath(options.ContentRootPath, AppContext.BaseDirectory);

//            this.hostingEnvironment.InitializeConsole(contentRootPath, options);
//            this.context.HostingEnvironment = this.hostingEnvironment;

//            var services = new ServiceCollection();
//            services.AddSingleton(this.options);
//            services.AddSingleton<IHostingEnvironment>(this.hostingEnvironment);
//            services.AddSingleton<Microsoft.Extensions.Hosting.IHostingEnvironment>(this.hostingEnvironment);
//            services.AddSingleton(this.context);

//            var builder = new ConfigurationBuilder()
//                .SetBasePath(contentRootPath)
//                .AddConfiguration(this.configuration);

//            foreach (var configureAppConfiguration in this.configureAppConfigurationBuilderDelegates)
//            {
//                configureAppConfiguration(this.context, builder);
//            }

//            var configuration = builder.Build();
//            services.AddSingleton<IConfiguration>(configuration);
//            this.context.Configuration = configuration;

//            var listener = new DiagnosticListener("Microsoft.AspNetCore");
//            services.AddSingleton(listener);
//            services.AddSingleton<DiagnosticSource>(listener);

//            services.AddScoped<IMiddlewareFactory, MiddlewareFactory>();
//            services.AddTransient<IStartupFilter, AutoRequestServicesStartupFilter>();
//            services.AddTransient<IApplicationBuilderFactory, ApplicationBuilderFactory>();
//            services.AddTransient<IServiceProviderFactory<IServiceCollection>, DefaultServiceProviderFactory>();
//            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
//            services.AddOptions();
//            services.AddLogging();

//            if (!string.IsNullOrEmpty(options.StartupAssembly))
//            {
//                try
//                {
//                    var startupType = StartupLoader.FindStartupType(this.options.StartupAssembly, this.hostingEnvironment.EnvironmentName);

//                    if (typeof(IStartup).GetTypeInfo().IsAssignableFrom(startupType.GetTypeInfo()))
//                    {
//                        services.AddSingleton(typeof(IStartup), startupType);
//                    }
//                    else
//                    {
//                        services.AddSingleton(typeof(IStartup), sp =>
//                        {
//                            var hostingEnvironment = sp.GetRequiredService<IHostingEnvironment>();
//                            var methods = StartupLoader.LoadMethods(sp, startupType, hostingEnvironment.EnvironmentName);
//                            return new ConventionBasedStartup(methods);
//                        });
//                    }
//                }
//                catch (Exception ex)
//                {
//                    var capture = ExceptionDispatchInfo.Capture(ex);
//                    services.AddSingleton<IStartup>(_ =>
//                    {
//                        capture.Throw();
//                        return null;
//                    });
//                }
//            }

//            foreach (var configureServices in this.configureServicesDelegates)
//            {
//                configureServices(context, services);
//            }

//            return services;
//        }
//        private void AddApplicationServices(IServiceCollection services, IServiceProvider hostingServiceProvider)
//        {
//            if (services == null)
//                throw new ArgumentNullException(nameof(services));

//            if (hostingServiceProvider == null)
//                throw new ArgumentNullException(nameof(hostingServiceProvider));

//            var listener = hostingServiceProvider.GetService<DiagnosticListener>();
//            services.Replace(ServiceDescriptor.Singleton(typeof(DiagnosticListener), listener));
//            services.Replace(ServiceDescriptor.Singleton(typeof(DiagnosticSource), listener));
//        }
//        private string ResolveContentRootPath(string contentRootPath, string basePath)
//        {
//            if (string.IsNullOrEmpty(contentRootPath))
//                return basePath;

//            if (Path.IsPathRooted(contentRootPath))
//                return contentRootPath;

//            return Path.Combine(Path.GetFullPath(basePath), contentRootPath);
//        }
//    }
//}