using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nano.Logging.Abstractions;
using Nano.Logging.Abstractions.Config;
using Nano.Logging.Microsoft.Extensions;

namespace Nano.Logging.Microsoft;

/// <summary>
/// A logging provider that configures the built-in Microsoft.Extensions.Logging framework.
/// </summary>
public class MicrosoftProvider : ILoggingProvider
{
    /// <summary>
    /// Configures Microsoft logging for the application using the provided <see cref="LoggingOptions"/>.
    /// <para>
    ///     This includes adding a simple console logger with timestamped output, setting the default minimum log level,
    ///     and applying namespace-specific log level overrides.
    /// </para>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register Microsoft logging services with.</param>
    /// <param name="options">The <see cref="LoggingOptions"/> controlling log levels and overrides.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="options"/> is <c>null</c>.</exception>
    public static void Configure(IServiceCollection services, LoggingOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services
            .AddLogging(x =>
            {
                x.AddSimpleConsole(y =>
                {
                    y.IncludeScopes = true;
                    y.TimestampFormat = "dd-MM-yyyy HH:mm:ss.ffffff ";
                });

                x.SetMinimumLevel(options.LogLevel.GetLogLevel());

                foreach (var @override in options.LogLevelOverrides)
                {
                    x.AddFilter((category, logLevel) =>
                    {
                        if (!(category?.StartsWith(@override.Namespace) ?? false))
                        {
                            return true;
                        }

                        var filterLogLevel = @override.LogLevel
                            .GetLogLevel();

                        return logLevel >= filterLogLevel;
                    });
                }
            });
    }
}