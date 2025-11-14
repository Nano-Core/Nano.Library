using Nano.Logging.Abstractions.Enums;

namespace Nano.Logging.Abstractions.Config;

/// <summary>
/// Log Level Override.
/// </summary>
public class LogLevelOverrideOptions
{
    /// <summary>
    /// Namespace.
    /// </summary>
    public virtual string Namespace { get; set; }

    /// <summary>
    /// Log Level.
    /// </summary>
    public virtual LogLevel LogLevel { get; set; } = LogLevel.Warning;
}