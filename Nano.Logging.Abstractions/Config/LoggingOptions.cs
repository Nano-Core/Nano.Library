using Nano.Logging.Abstractions.Enums;
using System.ComponentModel.DataAnnotations;

namespace Nano.Logging.Abstractions.Config;

/// <summary>
/// Represents the configuration options for logging in the application.
/// Includes the default log level and any namespace-specific overrides.
/// </summary>
public class LoggingOptions
{
    internal static string SectionName => "Logging";

    /// <summary>
    /// The default minimum <see cref="LogLevel"/> used by the logging provider.
    /// </summary>
    [Required]
    public virtual LogLevel LogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// Optional overrides for specific namespaces, allowing different log levels for different parts of the application.
    /// </summary>
    [Required]
    public virtual LogLevelOverrideOptions[] LogLevelOverrides { get; set; } = [];
}
