using Nano.Logging.Abstractions.Enums;
using System.ComponentModel.DataAnnotations;

namespace Nano.Logging.Abstractions.Config;

/// <summary>
/// Represents a log level override for a specific namespace.
/// </summary>
public class LogLevelOverrideOptions
{
    /// <summary>
    /// The namespace for which this log level override applies.
    /// </summary>
    [Required]
    public virtual string Namespace { get; set; } = null!;

    /// <summary>
    /// The <see cref="LogLevel"/> to use for the specified namespace.
    /// </summary>
    [Required]
    public virtual LogLevel LogLevel { get; set; } = LogLevel.Warning;
}