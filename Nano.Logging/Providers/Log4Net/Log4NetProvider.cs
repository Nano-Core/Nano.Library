using System;
using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.Logging;
using Nano.Logging.Interfaces;
using Nano.Logging.Providers.Log4Net.Extensions;

namespace Nano.Logging.Providers.Log4Net
{
    /// <inheritdoc />
    public class Log4NetProvider : ILoggingProvider
    {
        /// <summary>
        /// Options.
        /// </summary>
        protected virtual LoggingOptions Options { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">The <see cref="LoggingOptions"/>.</param>
        public Log4NetProvider(LoggingOptions options)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
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

            hierarchy.Root.Level = this.Options.LogLevel
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

            return  new Microsoft.Extensions.Logging.Log4NetProvider(providerOptions);
        }
    }
}