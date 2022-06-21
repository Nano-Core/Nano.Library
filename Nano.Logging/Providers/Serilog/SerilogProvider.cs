using System;
using Microsoft.Extensions.Logging;
using Nano.Logging.Interfaces;
using Nano.Logging.Providers.Serilog.Extensions;
using Serilog;
using Serilog.Extensions.Logging;

namespace Nano.Logging.Providers.Serilog;

/// <inheritdoc />
public class SerilogProvider : ILoggingProvider
{
    /// <summary>
    /// Options.
    /// </summary>
    protected virtual LoggingOptions Options { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="LoggingOptions"/>.</param>
    public SerilogProvider(LoggingOptions options)
    {
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public virtual ILoggerProvider Configure()
    {
        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Is(this.Options.LogLevel.GetLogLevel());

        loggerConfiguration
            .WriteTo.Console(outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.ffffff} [{Level:u3}] {Message}{NewLine}{Exception}");

        foreach (var @override in this.Options.LogLevelOverrides)
        {
            loggerConfiguration
                .MinimumLevel.Override(@override.Namespace, @override.LogLevel.GetLogLevel());
        }

        Log.Logger = loggerConfiguration
            .CreateLogger();

        return new SerilogLoggerProvider(Log.Logger, true);
    }
}