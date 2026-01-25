using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.Config;

/// <summary>
/// Defines culture and localization options for the application.
/// </summary>
public class CultureOptions
{
    /// <summary>
    /// The default culture used by the application.
    /// </summary>
    [Required]
    public virtual string Default { get; set; } = "en-US";

    /// <summary>
    /// The set of cultures supported by the application.
    /// </summary>
    [Required]
    public virtual IEnumerable<string> Supported { get; set; } = [];
}