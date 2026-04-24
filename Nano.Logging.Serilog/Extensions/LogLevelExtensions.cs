using Nano.Logging.Abstractions.Enums;
using Serilog.Events;

namespace Nano.Logging.Serilog.Extensions;

internal static class LogLevelExtensions
{
    internal static LogEventLevel GetLogLevel(this LogLevel logLevel)
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