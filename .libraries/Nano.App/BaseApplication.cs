using System;
using System.IO;
using System.Linq;
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
    public abstract class BaseApplication : IApplication
    {
        /// <summary>
        /// Configuration.
        /// </summary>
        protected virtual IConfiguration Configuration { get; set; }

        /// <summary>
        /// Constructor. 
        /// Accepting an instance of <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        protected BaseApplication(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            this.Configuration = configuration;
        }

        /// <inheritdoc />
        public abstract void ConfigureServices(IServiceCollection services);

        /// <inheritdoc />
        public abstract void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment, IApplicationLifetime applicationLifetime);
      
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
            var configuration = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile($"{NAME}.json", false, true)
                .AddJsonFile($"{NAME}.{environment}.json", true)
                .AddEnvironmentVariables()
                .Build();
            var shutdownTimeout = TimeSpan.FromSeconds(10);

            var options = configuration.GetSection(AppOptions.SectionName).Get<AppOptions>() ?? new AppOptions();
            var urls = options.Hosting.Ports.Select(x => $"http://*:{x}").Distinct().ToArray();

            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(path)
                .UseEnvironment(environment)
                .UseConfiguration(configuration)
                .UseShutdownTimeout(shutdownTimeout)
                .UseUrls(urls)
                .CaptureStartupErrors(true)
                .ConfigureServices(x =>
                {
                    x.AddApp(configuration);
                    x.AddData(configuration);
                    x.AddConfig(configuration);
                    x.AddLogging(configuration);
                    x.AddEventing(configuration);

                    x.AddSingleton<IApplication, TApplication>();
                })
                .UseStartup<TApplication>();
        }
    }
}
