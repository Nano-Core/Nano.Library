using Nano.Logging.Abstractions.Enums;

namespace Nano.Logging.NLog.Extensions;

/// <summary>
/// Log Level Extensions.
/// </summary>
public static class LogLevelExtensions
{
    /// <summary>
    /// Get Log-Level.
    /// </summary>
    /// <param name="logLevel">The <see cref="LogLevel"/>.</param>
    /// <returns>The <see cref="Microsoft.Extensions.Logging.LogLevel"/>.</returns>
    public static global::NLog.LogLevel GetLogLevel(this LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Debug => global::NLog.LogLevel.Debug,
            LogLevel.Information => global::NLog.LogLevel.Info,
            LogLevel.Warning => global::NLog.LogLevel.Warn,
            LogLevel.Error => global::NLog.LogLevel.Error,
            LogLevel.Fatal => global::NLog.LogLevel.Fatal,
            _ => global::NLog.LogLevel.Debug
        };
    }
}