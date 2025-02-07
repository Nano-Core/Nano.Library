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
    public virtual LogLevelOverride[] LogLevelOverrides { get; set; } = [];

    /// <summary>
    /// Log Level Override (nested class).
    /// </summary>
    public class LogLevelOverride
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

    /// <summary>
    /// Connection String.
    /// Optional, used by some logging providers.
    /// </summary>
    public virtual string ConnectionString { get; set; }
}