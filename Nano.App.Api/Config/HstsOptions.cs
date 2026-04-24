using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for HTTP Strict Transport Security (HSTS).
/// </summary>
public class HstsOptions
{
    /// <summary>
    /// Maximum age for HSTS.
    /// </summary>
    public virtual TimeSpan MaxAge { get; set; } = TimeSpan.FromDays(180);

    /// <summary>
    /// Enable or disable the preload directive.
    /// Preload will only used if <see cref="MaxAge"/> is greater than 7 weeks.
    /// </summary>
    [Required]
    public virtual bool UsePreload { get; set; } = false;

    /// <summary>
    /// Include subdomains in HSTS policy.
    /// </summary>
    [Required]
    public virtual bool IncludeSubdomains { get; set; } = false;
}
