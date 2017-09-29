using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Hosting.Middleware.Extensions;
using Nano.App.Logging.Options;
using Nano.App.Logging.Providers.Interfaces;
using Serilog;

namespace Nano.App.Logging.Extensions
{
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
        /// Adds logging provider of type <typeparamref name="TProvider"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TProvider">The <typeparamref name="TProvider"/> type.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddLogging<TProvider>(this IServiceCollection services)
            where TProvider : ILoggingProvider, new()
        {
            var provider = services.BuildServiceProvider();
            var loggingOptions = provider.GetRequiredService<LoggingOptions>();

            var loggerConfiguration = new LoggerConfiguration()
                .WriteTo.Sink<TProvider>()
                .MinimumLevel.Is(loggingOptions.LogLevel);

            foreach (var @override in loggingOptions.LogLevelOverrides)
            {
                loggerConfiguration
                    .MinimumLevel.Override(@override.Namespace, @override.LogLevel);
            }

            Log.Logger = loggerConfiguration.CreateLogger();

            return services;
        }
   }
}