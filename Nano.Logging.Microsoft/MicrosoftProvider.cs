using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nano.Logging.Abstractions;
using Nano.Logging.Abstractions.Config;
using Nano.Logging.Microsoft.Extensions;

namespace Nano.Logging.Microsoft;

/// <inheritdoc />
public class MicrosoftProvider : ILoggingProvider
{
    /// <inheritdoc />
    public virtual void Configure(IServiceCollection services, LoggingOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        services
            .AddLogging(x =>
            {
                x.AddSimpleConsole(y =>
                {
                    y.IncludeScopes = true;
                    y.TimestampFormat = "dd-MM-yyyy HH:mm:ss.ffffff ";
                });
                
                x.SetMinimumLevel(options.LogLevel.GetLogLevel());

                if (options.LogLevelOverrides != null)
                {
                    foreach (var @override in options.LogLevelOverrides)
                    {
                        x.AddFilter((category, logLevel) =>
                        {
                            if (category.StartsWith(@override.Namespace))
                            {
                                var filterLogLevel = @override.LogLevel
                                    .GetLogLevel();

                                return logLevel >= filterLogLevel;
                            }

                            return true;
                        });
                    }
                }
            });
    }
}