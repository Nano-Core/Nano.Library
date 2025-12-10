using System;
using Microsoft.Extensions.DependencyInjection;
using Nano.Logging.Abstractions;
using Nano.Logging.Abstractions.Config;
using Nano.Logging.Serilog.Extensions;
using Serilog;
using Serilog.Extensions.Logging;

namespace Nano.Logging.Serilog;

/// <inheritdoc />
public class SerilogProvider : ILoggingProvider
{
    /// <inheritdoc />
    public virtual void Configure(IServiceCollection services, LoggingOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null) 
            throw new ArgumentNullException(nameof(options));

        var a = new SerilogLoggerProvider();

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