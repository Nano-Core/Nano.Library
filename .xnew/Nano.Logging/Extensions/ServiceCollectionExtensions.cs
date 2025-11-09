using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nano.Config.Extensions;
using Nano.Logging.Interfaces;
using System;

namespace Nano.Logging.Extensions;

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
            .AddConfigSection<LoggingOptions>(LoggingOptions.SectionName, out var options);

        if (options == null)
        {
            return services;
        }

        services
            .AddSingleton<ILoggingProvider, TProvider>()
            .AddSingleton(x => x
                .GetRequiredService<ILoggingProvider>()
                .Configure())
            .AddSingleton<ILoggerFactory>(x =>
            {
                var loggerProvider = x
                    .GetRequiredService<ILoggerProvider>();

                return new LoggerFactory([loggerProvider]);
            })
            .AddSingleton(x =>
            {
                var loggerProvider = x
                    .GetRequiredService<ILoggerFactory>();

                return loggerProvider
                    .CreateLogger(string.Empty);
            });

        return services;
    }
}