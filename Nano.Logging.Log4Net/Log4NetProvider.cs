using System;
using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nano.Logging.Abstractions;
using Nano.Logging.Abstractions.Config;
using Nano.Logging.Log4Net.Extensions;

namespace Nano.Logging.Log4Net;

/// <inheritdoc />
public class Log4NetProvider : ILoggingProvider
{
    /// <inheritdoc />
    public virtual void Configure(IServiceCollection services, LoggingOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        var patternLayout = new PatternLayout
        {
            ConversionPattern = "%utcdate{dd-MM-yyyy HH:mm:ss.ffffff} [%level] %logger - %message%newline"
        };

        patternLayout
            .ActivateOptions();

        var consoleAppender = new ConsoleAppender
        {
            Layout = patternLayout
        };

        consoleAppender
            .ActivateOptions();

        var hierarchy = (Hierarchy)LogManager.CreateRepository("default");

        hierarchy.Root
            .AddAppender(consoleAppender);

        hierarchy.Root.Level = options.LogLevel.GetLogLevel();

        if (options.LogLevelOverrides != null)
        {
            foreach (var over in options.LogLevelOverrides)
            {
                if (hierarchy.GetLogger(over.Namespace) is Logger logger)
                {
                    logger.Level = over.LogLevel.GetLogLevel();

                    logger
                        .AddAppender(consoleAppender);
                }
            }
        }

        hierarchy.Configured = true;

        var providerOptions = new Log4NetProviderOptions
        {
            ExternalConfigurationSetup = true,
            LoggerRepository = hierarchy.Name
        };

        services
            .AddSingleton<ILoggerProvider>(new Microsoft.Extensions.Logging.Log4NetProvider(providerOptions));
    }
}