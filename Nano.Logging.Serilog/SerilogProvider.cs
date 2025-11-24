using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nano.Logging.Abstractions;
using Nano.Logging.Abstractions.Config;
using Nano.Logging.Serilog.Extensions;
using Serilog;
using Serilog.Extensions.Logging;

namespace Nano.Logging.Serilog;

/// <inheritdoc />
public class SerilogProvider : ILoggingProvider
{
    private readonly IOptionsMonitor<LoggingOptions> options;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="LoggingOptions"/>.</param>
    public SerilogProvider(IOptionsMonitor<LoggingOptions> options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public virtual ILoggerProvider Configure()
    {
        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Is(this.options.CurrentValue.LogLevel.GetLogLevel());

        loggerConfiguration
            .WriteTo.Console(outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.ffffff} [{Level:u3}] {Message}{NewLine}{Exception}");

        foreach (var @override in this.options.CurrentValue.LogLevelOverrides)
        {
            loggerConfiguration
                .MinimumLevel.Override(@override.Namespace, @override.LogLevel.GetLogLevel());
        }

        Log.Logger = loggerConfiguration
            .CreateLogger();

        return new SerilogLoggerProvider(Log.Logger, true);
    }
}