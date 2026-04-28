using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nano.Logging.Abstractions;
using Nano.Logging.Abstractions.Config;
using System;
using Nano.Common.Config.Extensions;

namespace Nano.Logging.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register Nano services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a Nano logging provider of type <typeparamref name="TProvider"/> with the <see cref="IServiceCollection"/>.
    /// Configures the provider using the <see cref="LoggingOptions"/> section from the application's configuration.
    /// </summary>
    /// <typeparam name="TProvider">The type of the logging provider to register. Must implement <see cref="ILoggingProvider"/> and have a parameterless constructor.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the logging provider to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    /// <remarks>Documentation: https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging/README.md#nanologging.</remarks>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <c>null</c>.</exception>
    public static IServiceCollection AddNanoLogging<TProvider>(this IServiceCollection services)
        where TProvider : ILoggingProvider
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddNanoConfigSection<LoggingOptions>(LoggingOptions.SectionName, out var options);

        if (options is null)
        {
            throw new InvalidOperationException($"Configuration section '{LoggingOptions.SectionName}' could not be loaded.");
        }

        TProvider.Configure(services, options);

        services
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