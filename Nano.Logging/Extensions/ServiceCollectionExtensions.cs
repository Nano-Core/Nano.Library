using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nano.Config.Extensions;
using Nano.Logging.Interfaces;

namespace Nano.Logging.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds logging provider of type <typeparamref name="TProvider"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TProvider">The <typeparamref name="TProvider"/> type.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddLogging<TProvider>(this IServiceCollection services)
            where TProvider : class, ILoggingProvider
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddSingleton<ILoggingProvider, TProvider>()
                .AddSingleton(x => x
                    .GetRequiredService<ILoggingProvider>()
                    .Configure())
                .AddSingleton(x =>
                {
                    var loggerProvider = x.GetRequiredService<ILoggerProvider>();

                    return loggerProvider.CreateLogger(null);
                })
                .AddSingleton<ILoggerFactory>(x =>
                {
                    var loggerProvider = x.GetRequiredService<ILoggerProvider>();

                    return new LoggerFactory(new[] { loggerProvider });
                });

            return services;
        }

        /// <summary>
        /// Adds <see cref="LoggingOptions"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            return services
                .AddConfigOptions<LoggingOptions>(configuration, LoggingOptions.SectionName, out var _);
        }
    }
}