using Nano.Logging.Abstractions.Enums;
using System.ComponentModel.DataAnnotations;

namespace Nano.Logging.Abstractions.Config;

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
    [Required]
    public virtual LogLevel LogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// Log Level Overrides.
    /// </summary>
    [Required]
    public virtual LogLevelOverrideOptions[] LogLevelOverrides { get; set; } = [];
}
