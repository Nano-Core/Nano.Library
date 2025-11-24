using System;
using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nano.Logging.Abstractions;
using Nano.Logging.Abstractions.Config;
using Nano.Logging.Log4Net.Extensions;

namespace Nano.Logging.Log4Net;

/// <inheritdoc />
public class Log4NetProvider : ILoggingProvider
{
    private readonly IOptionsMonitor<LoggingOptions> options;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="LoggingOptions"/>.</param>
    public Log4NetProvider(IOptionsMonitor<LoggingOptions> options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public virtual ILoggerProvider Configure()
    {
        var patternLayout = new PatternLayout
        {
            ConversionPattern = "%utcdate{dd-MM-yyyy HH:mm:ss.ffffff} [%level] %message%newline"
        };
        patternLayout.ActivateOptions();

        var consoleAppender = new ConsoleAppender
        {
            Layout = patternLayout
        };

        var hierarchy = (Hierarchy)LogManager
            .CreateRepository("default");

        hierarchy.Root
            .AddAppender(consoleAppender);

        hierarchy.Root.Level = this.options.CurrentValue.LogLevel
            .GetLogLevel();

        hierarchy.Configured = true;

        // TODO: Log4Net: Namespace overrides.
        //foreach (var logLevelOverride in this.Options.LogLevelOverrides)
        //{
        //    var nestedHierarchy = (Hierarchy)LogManager
        //        .CreateRepository(logLevelOverride.Namespace);

        //    nestedHierarchy.Root.Level = logLevelOverride.LogLevel
        //        .GetLogLevel();

        //    nestedHierarchy.Root
        //        .AddAppender(consoleAppender);

        //    nestedHierarchy.Configured = true;
        //}

        var providerOptions = new Log4NetProviderOptions
        {
            ExternalConfigurationSetup = true,
            LoggerRepository = hierarchy.Name
        };

        return new Microsoft.Extensions.Logging.Log4NetProvider(providerOptions);
    }
}