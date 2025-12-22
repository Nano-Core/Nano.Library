using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.Config;

/// <summary>
/// Culture Options.
/// </summary>
public class CultureOptions
{
    /// <summary>
    /// Default.
    /// </summary>
    [Required]
    public virtual string Default { get; set; } = "en-US";

    /// <summary>
    /// Supported.
    /// </summary>
    [Required]
    public virtual IEnumerable<string> Supported { get; set; } = [];
}