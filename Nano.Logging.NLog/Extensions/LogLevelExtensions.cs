using Nano.Logging.Abstractions.Enums;

namespace Nano.Logging.NLog.Extensions;

internal static class LogLevelExtensions
{
    internal static global::NLog.LogLevel GetLogLevel(this LogLevel logLevel)
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