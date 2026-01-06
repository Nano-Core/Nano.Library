using System;
using Microsoft.Extensions.DependencyInjection;
using Nano.Logging.Abstractions;
using Nano.Logging.Abstractions.Config;
using Nano.Logging.NLog.Extensions;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;

namespace Nano.Logging.NLog;

/// <inheritdoc />
public class NLogProvider : ILoggingProvider
{
    /// <inheritdoc />
    public virtual void Configure(IServiceCollection services, LoggingOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        var configuration = new LoggingConfiguration();
        var target = new ConsoleTarget("console")
        {
            Layout = "${longdate} [${level:uppercase=true}] ${logger} - ${message}${exception:format=toString}"
        };

        configuration
            .AddTarget(target);

        configuration.LoggingRules
            .Add(new LoggingRule("*", options.LogLevel.GetLogLevel(), target));

        if (options.LogLevelOverrides != null)
        {
            foreach (var @override in options.LogLevelOverrides)
            {
                configuration.LoggingRules
                    .Add(new LoggingRule($"{@override.Namespace}.*", @override.LogLevel.GetLogLevel(), target));
            }
        }

        services
            .AddLogging(x =>
            {
                x.AddNLog(configuration);
            });

        LogManager.Configuration = configuration;
    }
}