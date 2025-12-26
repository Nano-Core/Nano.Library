using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Nano.Common.Config.Extensions;
using Nano.Logging.Abstractions;
using Nano.Logging.Abstractions.Config;

namespace Nano.Logging.Extensions;

// TODO: Make Microsoft Logging Console Provider
// TODO: Make Nlog Logging Console Provider
// TODO: Improve logging from Chat-gPT serilog
// - A “production-grade” Serilog template
// - Structured logging examples
// - Logging with OpenTelemetry + Serilog
// - Advice on filtering noisy logs

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
    public static IServiceCollection AddNanoLogging<TProvider>(this IServiceCollection services)
        where TProvider : class, ILoggingProvider, new()
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddConfigSection<LoggingOptions>(LoggingOptions.SectionName, out var options);

        if (options == null)
        {
            throw new NullReferenceException(nameof(options));
        }

        var provider = Activator.CreateInstance<TProvider>();
        provider
            .Configure(services, options);

        services
            .AddSingleton<ILoggingProvider>(provider)
            .AddSingleton(x =>
            {
                var loggerFactory = x
                    .GetRequiredService<ILoggerFactory>();

                return loggerFactory
                    .CreateLogger(string.Empty);
            });

        return services;
    }
}