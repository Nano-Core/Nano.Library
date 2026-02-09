using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for localization.
/// </summary>
public class LocalizationOptions
{
    /// <summary>
    /// The default culture used by the application.
    /// </summary>
    [Required]
    public virtual string DefaultCulture { get; set; } = "en-US";

    /// <summary>
    /// The set of cultures supported by the application.
    /// </summary>
    [Required]
    public virtual IEnumerable<string> SupportedCultures { get; set; } = [];
}