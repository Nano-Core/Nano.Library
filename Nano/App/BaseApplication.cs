using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Extensions;
using Nano.App.Interfaces;

namespace Nano.App
{
    /// <summary>
    /// Base Application (abstract).
    /// </summary>
    /// <typeparam name="TConfig">The type of <see cref="IConfiguration"/>.</typeparam>
    public abstract class BaseApplication<TConfig> : IApplication
        where TConfig : IConfiguration
    {
        /// <summary>
        /// Configuration.
        /// The <see cref="IConfiguration"/> instance of type <typeparamref name="TConfig"/>.
        /// </summary>
        protected virtual TConfig Configuration { get; set; }

        /// <summary>
        /// Constructor. 
        /// Accepting a <typeparamref name="TConfig"/> instance, initializing <see cref="Configuration"/>.
        /// </summary>
        /// <param name="configuration">The instance of <typeparamref name="TConfig"/>.</param>
        protected BaseApplication(TConfig configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            this.Configuration = configuration;
        }

        /// <summary>
        /// Configure Services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="hostingEnvironment">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="applicationLifetime">The <see cref="IApplicationBuilder"/>.</param>
        public virtual void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment, IApplicationLifetime applicationLifetime)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));

            if (applicationLifetime == null)
                throw new ArgumentNullException(nameof(applicationLifetime));

            applicationBuilder
                .UseHosting()
                .UseDataContext();
        }

        /// <summary>
        /// Creates a <see cref="IWebHostBuilder"/>, ready to <see cref="IWebHostBuilder.Build()"/> and <see cref="WebHostExtensions.Run(IWebHost)"/>.
        /// </summary>
        /// <typeparam name="TApplication">The type containing method for application start-up.</typeparam>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder ConfigureApp<TApplication>()
            where TApplication : class, IApplication
        {
            const string NAME = "appsettings";

            var path = Directory.GetCurrentDirectory();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var shutdownTimeout = TimeSpan.FromSeconds(10);
            var configuration =  new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile($"{NAME}.json", false, true)
                .AddJsonFile($"{NAME}.{environment}.json", true)
                .AddEnvironmentVariables()
                .Build();

            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(path)
                .UseLogging(configuration)
                .UseEnvironment(environment)
                .UseShutdownTimeout(shutdownTimeout)
                .CaptureStartupErrors(true)
                .ConfigureServices(x =>
                {
                    x.AddConfig(configuration);
                    x.AddLogging(configuration);
                    x.AddHosting(configuration);
                    x.AddEventing(configuration);
                    x.AddDataContext(configuration);
                })
                .UseApplication<TApplication>(configuration);
        }
    }
}
