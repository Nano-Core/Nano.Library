using Microsoft.OpenApi;
using Nano.App.Api.Config.Enums;
using Nano.App.Api.Mvc.Documentation.Consts;
using Nano.App.Config;
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
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// URL to which the web-hook will send requests.
    /// </summary>
    [Required]
    [Url]
    public virtual string Uri { get; set; } = null!;

    /// <summary>
    /// Optional payload to include in the web-hook request.
    /// </summary>
    public virtual string? Payload { get; set; }
}