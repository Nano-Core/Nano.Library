using Nano.Logging.Abstractions.Enums;
using System.ComponentModel.DataAnnotations;

namespace Nano.Logging.Abstractions.Config;

/// <summary>
/// Log Level Override.
/// </summary>
public class LogLevelOverrideOptions
{
    /// <summary>
    /// Namespace.
    /// </summary>
    [Required]
    public virtual string Namespace { get; set; }

    /// <summary>
    /// Log Level.
    /// </summary>
    [Required]
    public virtual LogLevel LogLevel { get; set; } = LogLevel.Warning;
}