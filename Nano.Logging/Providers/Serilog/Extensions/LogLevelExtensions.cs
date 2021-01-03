using Serilog.Events;

namespace Nano.Logging.Providers.Serilog.Extensions
{
    /// <summary>
    /// Serilog Log-Level Extensions.
    /// </summary>
    public static class SerilogLogLevelExtensions
    {
        /// <summary>
        /// Get Log-Level.
        /// </summary>
        /// <param name="logLevel">The <see cref="LogLevel"/>.</param>
        /// <returns>The <see cref="LogEventLevel"/>.</returns>
        public static LogEventLevel GetLogLevel(this LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Debug => LogEventLevel.Debug,
                LogLevel.Information => LogEventLevel.Information,
                LogLevel.Warning => LogEventLevel.Warning,
                LogLevel.Error => LogEventLevel.Error,
                LogLevel.Fatal => LogEventLevel.Fatal,
                _ => LogEventLevel.Debug
            };
        }
    }
}