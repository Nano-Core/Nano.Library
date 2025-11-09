namespace Nano.Logging;

/// <summary>
/// Logging Options.
/// </summary>
public class LoggingOptions
{
    /// <summary>
    /// Section Name.
    /// </summary>
    public static string SectionName => "Logging";

    /// <summary>
    /// Log Level.
    /// </summary>
    public virtual LogLevel LogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// Log Level Overrides.
    /// </summary>
    public virtual LogLevelOverrideOptions[] LogLevelOverrides { get; set; } = [];
}