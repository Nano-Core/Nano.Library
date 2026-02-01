using System.ComponentModel.DataAnnotations;

namespace Nano.App.Config;

/// <summary>
/// Options for localization.
/// </summary>
public class LocalizationOptions
{
    /// <summary>
    /// Culture and localization settings for the application.
    /// </summary>
    [Required]
    public virtual CultureOptions Cultures { get; set; } = new();
}