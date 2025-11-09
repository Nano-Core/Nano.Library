namespace Nano.Logging;

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