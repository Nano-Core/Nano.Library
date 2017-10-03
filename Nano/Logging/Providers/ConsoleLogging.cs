using System;
using System.IO;
using System.Text;
using Nano.Logging.Providers.Interfaces;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using Serilog.Sinks.SystemConsole.Themes;

namespace Nano.Logging.Providers
{
    /// <summary>
    /// Console Logging.
    /// </summary>
    public class ConsoleLogging : ILoggingProvider
    {
        private const int DEFAULT_WRITE_BUFFER = 256;
        private readonly object syncRoot = new object();
        private readonly ConsoleTheme theme = ConsoleTheme.None;
        private readonly LogEventLevel logLevel = LogEventLevel.Warning;
        private readonly ITextFormatter formatter = new JsonFormatter();

        /// <summary>
        /// Constructor.
        /// </summary>
        public ConsoleLogging()
        {
            
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="theme">the <see cref="ConsoleTheme"/>.</param>
        /// <param name="formatter">the <see cref="ITextFormatter"/>.</param>
        /// <param name="logLevel">the <see cref="LogEventLevel"/>.</param>
        public ConsoleLogging(ConsoleTheme theme, ITextFormatter formatter, LogEventLevel logLevel)
        {
            if (theme == null)
                throw new ArgumentNullException(nameof(theme));

            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            this.theme = theme;
            this.formatter = formatter;
            this.logLevel = logLevel;
        }

        /// <inheritdoc />
        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null)
                throw new ArgumentNullException(nameof(logEvent));

            var output = logEvent.Level < this.logLevel 
                ? Console.Out 
                : Console.Error;

            if (theme.CanBuffer)
            {
                var buffer = new StringWriter(new StringBuilder(ConsoleLogging.DEFAULT_WRITE_BUFFER));
                this.formatter.Format(logEvent, buffer);

                lock (syncRoot)
                {
                    output.Write(buffer.ToString());
                    output.Flush();
                }
            }
            else
            {
                lock (syncRoot)
                {
                    this.formatter.Format(logEvent, output);
                    output.Flush();
                }
            }
        }
    }
}