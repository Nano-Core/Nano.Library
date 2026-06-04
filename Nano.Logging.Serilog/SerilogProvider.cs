using System;
using Microsoft.Extensions.DependencyInjection;
using Nano.Logging.Abstractions;
using Nano.Logging.Abstractions.Config;
using Nano.Logging.Serilog.Extensions;
using Serilog;

namespace Nano.Logging.Serilog;

/// <summary>
/// A logging provider that configures Serilog as the application's logging framework.
/// </summary>
/// <remarks>Documentation: https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging.Serilog/README.md#nanologgingserilog</remarks>
public sealed class SerilogProvider : ILoggingProvider
{
    /// <summary>
    /// Configures Serilog logging for the application using the provided <see cref="LoggingOptions"/>.
    /// <para>
    ///     This includes setting the minimum log level, applying namespace-specific overrides, enriching the log context,
    ///     and writing logs to the console with a timestamped output format.
    /// </para>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register Serilog services with.</param>
    /// <param name="options">The <see cref="LoggingOptions"/> controlling log levels and overrides.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="options"/> is <c>null</c>.</exception>
    public static void Configure(IServiceCollection services, LoggingOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services
            .AddSerilog(x =>
            {
                x.Enrich
                    .FromLogContext();

                x.MinimumLevel
                    .Is(options.LogLevel.GetLogLevel());

                x.WriteTo
                    .Console(outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.ffffff} [{Level:u3}] {Message}{NewLine}{Exception}");

                foreach (var @override in options.LogLevelOverrides)
                {
                    x.MinimumLevel
                        .Override(@override.Namespace, @override.LogLevel.GetLogLevel());
                }
            });
    }
}