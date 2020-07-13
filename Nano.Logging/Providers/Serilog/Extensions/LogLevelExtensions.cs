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
            switch (logLevel)
            {
                case LogLevel.Debug:
                    return LogEventLevel.Debug;
                
                case LogLevel.Information:
                    return LogEventLevel.Information;
                
                case LogLevel.Warning:
                    return LogEventLevel.Warning;
                
                case LogLevel.Error:
                    return LogEventLevel.Error;
                
                case LogLevel.Fatal:
                    return LogEventLevel.Fatal;

                default:
                    return LogEventLevel.Debug;
            }
        }
    }
}