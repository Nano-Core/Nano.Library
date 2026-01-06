using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nano.Logging.Abstractions;
using Nano.Logging.Abstractions.Config;
using System;
using Nano.Common.Config.Extensions;

namespace Nano.Logging.Extensions;

// BUG: Make Microsoft Logging Console Provider
// BUG: Make Nlog Logging Console Provider

// BUG: Logging and Observability and Telemetry (OpenTelemetry - https://opentelemetry.io/docs)
// - Logging with OpenTelemetry + Serilog
// - IHostApplicationBuilder.Metrics(IMetricsBuilder)

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
            .AddNanoConfigSection<LoggingOptions>(LoggingOptions.SectionName, out var options);

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