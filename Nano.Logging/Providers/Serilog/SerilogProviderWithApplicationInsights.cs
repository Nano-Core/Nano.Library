using System;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;
using Nano.Logging.Interfaces;
using Nano.Logging.Providers.Serilog.Extensions;
using Serilog;
using Serilog.Extensions.Logging;

namespace Nano.Logging.Providers.Serilog;

/// <inheritdoc />
public class SerilogProviderWithApplicationInsights : ILoggingProvider
{
    /// <summary>
    /// Options.
    /// </summary>
    protected virtual LoggingOptions Options { get; }

    /// <summary>
    /// Telemetry Configuration.
    /// </summary>
    public virtual TelemetryConfiguration TelemetryConfiguration { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="LoggingOptions"/>.</param>
    /// <param name="telemetryConfiguration">The <see cref="TelemetryConfiguration"/></param>
    public SerilogProviderWithApplicationInsights(LoggingOptions options, TelemetryConfiguration telemetryConfiguration)
    {
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
        this.TelemetryConfiguration = telemetryConfiguration ?? throw new ArgumentNullException(nameof(telemetryConfiguration));
    }

    /// <inheritdoc />
    public virtual ILoggerProvider Configure()
    {
        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Is(this.Options.LogLevel.GetLogLevel());

        loggerConfiguration
            .WriteTo
            .Console(outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.ffffff} [{Level:u3}] {Message}{NewLine}{Exception}");

        if (this.Options.ConnectionString != null)
        {
            loggerConfiguration
                .WriteTo
                .ApplicationInsights(this.TelemetryConfiguration, TelemetryConverter.Events);
        }

        foreach (var @override in Options.LogLevelOverrides)
        {
            loggerConfiguration
                .MinimumLevel.Override(@override.Namespace, @override.LogLevel.GetLogLevel());
        }

        Log.Logger = loggerConfiguration
            .CreateLogger();

        return new SerilogLoggerProvider(Log.Logger, true);
    }
}