namespace Nano.Logging.Abstractions.Enums;

/// <summary>
/// Represents the severity level of a log message.
/// Used by logging providers to determine which messages should be recorded or displayed.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Debug-level messages, typically used for detailed diagnostic information.
    /// </summary>
    Debug,

    /// <summary>
    /// Informational messages that highlight the progress of the application.
    /// </summary>
    Information,

    /// <summary>
    /// Warning messages indicating a potential issue or important event that does not stop program execution.
    /// </summary>
    Warning,

    /// <summary>
    /// Error messages indicating a failure that prevents a specific operation from completing.
    /// </summary>
    Error,

    /// <summary>
    /// Fatal errors representing critical failures that cause the application to terminate.
    /// </summary>
    Fatal
}