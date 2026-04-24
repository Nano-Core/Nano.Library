using log4net.Core;
using Nano.Logging.Abstractions.Enums;

namespace Nano.Logging.Log4Net.Extensions;

internal static class Log4NetLogLevelExtensions
{
    internal static Level GetLogLevel(this LogLevel logLevel)
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