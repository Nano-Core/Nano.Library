using log4net.Core;

namespace Nano.Logging.Providers.Log4Net.Extensions;

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
        return logLevel switch
        {
            LogLevel.Debug => Level.Debug,
            LogLevel.Information => Level.Info,
            LogLevel.Warning => Level.Warn,
            LogLevel.Error => Level.Error,
            LogLevel.Fatal => Level.Fatal,
            _ => Level.Debug
        };
    }
}