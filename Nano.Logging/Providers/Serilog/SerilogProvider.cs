using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nano.Logging.Interfaces;
using Serilog;
using Serilog.Extensions.Logging;

namespace Nano.Logging.Providers.Serilog
{
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
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            this.Options = options;
        }

        /// <inheritdoc />
        public virtual ILoggerProvider Configure()
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Is(this.Options.LogLevel);

            var console = this.Options.Sinks?.FirstOrDefault(x => x?.Name?.ToLower() == "console");
            if (console != null)
            {
                loggerConfiguration
                    .WriteTo.Console();
            }

            var logstash = this.Options.Sinks?.FirstOrDefault(x => x?.Name?.ToLower() == "logstash");
            if (logstash != null)
            {
                loggerConfiguration
                    .WriteTo.LogstashHttp(logstash.ConnectionString);
            }

            loggerConfiguration
                .Enrich.FromLogContext();

            foreach (var @override in this.Options.LogLevelOverrides)
            {
                loggerConfiguration
                    .MinimumLevel.Override(@override.Namespace, @override.LogLevel);
            }
            
            Log.Logger = loggerConfiguration.CreateLogger();

            return new SerilogLoggerProvider(Log.Logger, true);
        }
    }
}