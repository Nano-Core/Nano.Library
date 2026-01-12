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

/// <summary>
/// A logging provider that configures log4net as the application's logging framework.
/// </summary>
public class Log4NetProvider : ILoggingProvider
{
    /// <summary>
    /// Configures log4net logging for the application using the provided <see cref="LoggingOptions"/>.
    /// <para>
    ///     This includes creating a console appender with a timestamped output format, setting the default minimum log level,
    ///     applying namespace-specific log level overrides, and registering log4net with the <see cref="IServiceCollection"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register log4net services with.</param>
    /// <param name="options">The <see cref="LoggingOptions"/> controlling log levels and overrides.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="options"/> is <c>null</c>.</exception>
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

        foreach (var over in options.LogLevelOverrides)
        {
            if (hierarchy.GetLogger(over.Namespace) is Logger logger)
            {
                logger.Level = over.LogLevel.GetLogLevel();

                logger
                    .AddAppender(consoleAppender);
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