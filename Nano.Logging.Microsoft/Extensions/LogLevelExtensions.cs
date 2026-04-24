using Nano.Logging.Abstractions.Enums;

namespace Nano.Logging.Microsoft.Extensions;

internal static class LogLevelExtensions
{
    internal static global::Microsoft.Extensions.Logging.LogLevel GetLogLevel(this LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Debug => global::Microsoft.Extensions.Logging.LogLevel.Debug,
            LogLevel.Information => global::Microsoft.Extensions.Logging.LogLevel.Information,
            LogLevel.Warning => global::Microsoft.Extensions.Logging.LogLevel.Warning,
            LogLevel.Error => global::Microsoft.Extensions.Logging.LogLevel.Error,
            LogLevel.Fatal => global::Microsoft.Extensions.Logging.LogLevel.Critical,
            _ => global::Microsoft.Extensions.Logging.LogLevel.Debug
        };
    }
}