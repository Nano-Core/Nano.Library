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
    public virtual TimeSpan? MaxAge { get; set; }

    /// <summary>
    /// Enable or disable the preload directive.
    /// </summary>
    [Required]
    public virtual bool UsePreload { get; set; } = false;

    /// <summary>
    /// Include subdomains in HSTS policy.
    /// </summary>
    [Required]
    public virtual bool IncludeSubdomains { get; set; } = false;
}
