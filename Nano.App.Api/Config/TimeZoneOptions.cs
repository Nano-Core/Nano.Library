using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for timezone.
/// </summary>
public class TimeZoneOptions
{
    /// <summary>
    /// Default time zone for the application.
    /// </summary>
    [Required]
    public virtual string DefaultTimeZone { get; set; } = "UTC";
}