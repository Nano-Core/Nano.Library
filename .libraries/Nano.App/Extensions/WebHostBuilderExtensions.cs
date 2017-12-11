using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Interfaces;
using Nano.Logging;
using Serilog;

namespace Nano.App.Extensions
{
    /// <summary>
    /// Web Host Builder Extensions.
    /// </summary>
    internal static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Use Logging.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        internal static IWebHostBuilder UseLogging(this IWebHostBuilder builder, IConfiguration configuration)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var options = configuration
                .GetSection(LoggingOptions.SectionName)
                .Get<LoggingOptions>();

            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Is(options.LogLevel);

            if (options.Sinks.Any(x => x == "Console"))
                loggerConfiguration.WriteTo.Console();

            if (options.Sinks.Any(x => x == "ElasticSearch"))
                loggerConfiguration.WriteTo.Elasticsearch();

            foreach (var @override in options.LogLevelOverrides)
            {
                loggerConfiguration
                    .MinimumLevel.Override(@override.Namespace, @override.LogLevel);
            }

            Log.Logger = loggerConfiguration.CreateLogger();

            return builder;
        }

        /// <summary>
        /// Use Application.
        /// </summary>
        /// <typeparam name="TApplication">The type of <see cref="IApplication"/>.</typeparam>
        /// <param name="builder">The <see cref="IWebHostBuilder"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        internal static IWebHostBuilder UseApplication<TApplication>(this IWebHostBuilder builder, IConfiguration configuration)
            where TApplication : class, IApplication
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var options = configuration
                .GetSection(AppOptions.SectionName)
                .Get<AppOptions>();

            var urls = options.Hosting.Ports
                .Select(x => $"http://*:{x}")
                .Distinct()
                .ToArray();

            return builder
                .ConfigureServices(x =>
                {
                    x.AddSingleton<IApplication, TApplication>();
                })
                .UseUrls(urls)
                .UseStartup<TApplication>();
        }
    }
}