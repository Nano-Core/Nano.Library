using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Logging.Middleware.Extensions;
using Nano.App.Logging.Options;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Nano.App.Logging.Extensions
{
    // TODO: Fix how Providers are added (Like logging and the difference between ElasticSearch and Console in for examppe congiguration properties.
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds logging to the <see cref="IServiceCollection"/>.
        /// Configures <see cref="LoggingOptions"/> for the passed <paramref name="configuration"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var section = configuration.GetSection("Logging");
            var options = section?.Get<LoggingOptions>() ?? new LoggingOptions();

            services
                .AddSingleton(options)
                .Configure<LoggingOptions>(section);

            if (options.IncludeHttpContext)
                services.AddHttpContext();

            return services;
        }

        /// <summary>
        /// Adds console logging to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddLoggingDebug(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var provider = services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();
            var section = configuration.GetSection("Logging");
            var options = section.Get<LoggingOptions>();

            return services
                .AddLogging(builder =>
                {
                    var loggerConfiguration = new LoggerConfiguration()
                        .MinimumLevel.Is(options.LogLevel);

                    if (options.IncludeLogContext)
                    {
                        loggerConfiguration
                            .Enrich.FromLogContext();
                    }

                    foreach (var @override in options.LogLevelOverrides)
                    {
                        loggerConfiguration
                            .MinimumLevel.Override(@override.Namespace, @override.LogLevel);
                    }

                    var logger = loggerConfiguration
                        .WriteTo.Debug()
                        .CreateLogger();

                    builder.AddSerilog(logger, true);
                });
        }

        /// <summary>
        /// Adds console logging to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddLoggingConsole(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var provider = services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();
            var section = configuration.GetSection("Logging");
            var options = section.Get<LoggingOptions>();

            return services
                .AddLogging(builder =>
                {
                    var loggerConfiguration = new LoggerConfiguration()
                        .MinimumLevel.Is(options.LogLevel);

                    if (options.IncludeLogContext)
                    {
                        loggerConfiguration
                            .Enrich.FromLogContext();
                    }

                    foreach (var @override in options.LogLevelOverrides)
                    {
                        loggerConfiguration
                            .MinimumLevel.Override(@override.Namespace, @override.LogLevel);
                    }

                    var logger = loggerConfiguration
                        .WriteTo.Console()
                        .CreateLogger();

                    builder.AddSerilog(logger, true);
                });
        }

        /// <summary>
        /// Adds elastic Search logging to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection UseLoggingElasticSearch(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var provider = services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();
            var section = configuration.GetSection("Logging");
            var options = section.Get<ElasticSearchLoggingOptions>() ?? new ElasticSearchLoggingOptions();

            return services
                .AddSingleton(options)
                .AddLogging(builder =>
                {
                    var loggerConfiguration = new LoggerConfiguration()
                        .MinimumLevel.Is(options.LogLevel);

                    if (options.IncludeLogContext)
                    {
                        loggerConfiguration
                            .Enrich.FromLogContext();
                    }

                    foreach (var @override in options.LogLevelOverrides)
                    {
                        loggerConfiguration
                            .MinimumLevel.Override(@override.Namespace, @override.LogLevel);
                    }

                    var logger = loggerConfiguration
                        .WriteTo.Elasticsearch(
                            new ElasticsearchSinkOptions(options.NodeUris.Select(x => new Uri(x)))
                            {
                                MinimumLogEventLevel = options.LogLevel,
                                AutoRegisterTemplate = options.AutoRegisterTemplate,
                                TemplateName = options.TemplateName,
                                ModifyConnectionSettings = options.ModifyConnectionSettings,
                                IndexFormat = options.IndexFormat,
                                TypeName = options.TypeName,
                                BatchPostingLimit = options.BatchPostingLimit,
                                Period = new TimeSpan(options.Period),
                                FormatProvider = options.FormatProvider,
                                Connection = options.Connection,
                                ConnectionTimeout = new TimeSpan(options.ConnectionTimeout),
                                InlineFields = options.InlineFields,
                                Serializer = options.Serializer,
                                IndexDecider = options.IndexDecider,
                                BufferBaseFilename = options.BufferBaseFilename,
                                BufferFileSizeLimitBytes = options.BufferFileSizeLimitBytes,
                                BufferLogShippingInterval = new TimeSpan(options.BufferLogShippingInterval),
                                CustomFormatter = options.CustomFormatter,
                                CustomDurableFormatter = options.CustomDurableFormatter
                            })
                        .CreateLogger();

                    builder.AddSerilog(logger, true);
                });
        }
   }
}