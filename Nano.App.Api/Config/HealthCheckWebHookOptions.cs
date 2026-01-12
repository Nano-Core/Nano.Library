using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Health-Check Web-Hook Options.
/// </summary>
public class HealthCheckWebHookOptions
{
    /// <summary>
    /// Name.
    /// </summary>
    [Required]
    public virtual string Name { get; set; }

    /// <summary>
    /// Uri.
    /// </summary>
    [Required]
    [Url]
    public virtual string Uri { get; set; }

    /// <summary>
    /// Payload.
    /// </summary>
    public virtual string Payload { get; set; }
}