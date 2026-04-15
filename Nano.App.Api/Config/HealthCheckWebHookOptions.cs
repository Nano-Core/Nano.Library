using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for configuring health-check web-hooks.
/// </summary>
public class HealthCheckWebHookOptions
{
    /// <summary>
    /// Name of the web-hook.
    /// </summary>
    [Required]
    public virtual required string Name { get; set; }

    /// <summary>
    /// URL to which the web-hook will send requests.
    /// </summary>
    [Required]
    [Url]
    public virtual required string Uri { get; set; }

    /// <summary>
    /// Optional payload to include in the web-hook request.
    /// </summary>
    public virtual string? Payload { get; set; }
}