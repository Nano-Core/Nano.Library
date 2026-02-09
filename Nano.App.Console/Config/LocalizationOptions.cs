using System.ComponentModel.DataAnnotations;

namespace Nano.App.Console.Config;

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
}