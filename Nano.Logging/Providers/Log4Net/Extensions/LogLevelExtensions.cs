using log4net.Core;

namespace Nano.Logging.Providers.Log4Net.Extensions
{
    /// <summary>
    /// Log4Net Log-Level Extensions.
    /// </summary>
    public static class Log4NetLogLevelExtensions
    {
        /// <summary>
        /// Get Log-Level.
        /// </summary>
        /// <param name="logLevel">The <see cref="LogLevel"/>.</param>
        /// <returns>The <see cref="Level"/>.</returns>
        public static Level GetLogLevel(this LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    return Level.Debug;
                
                case LogLevel.Information:
                    return Level.Info;
                
                case LogLevel.Warning:
                    return Level.Warn;
                
                case LogLevel.Error:
                    return Level.Error;

                case LogLevel.Fatal:
                    return Level.Fatal;

                default:
                    return Level.Debug;
            }
        }
    }
}