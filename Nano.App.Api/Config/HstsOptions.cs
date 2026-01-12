using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Hsts Options
/// </summary>
public class HstsOptions
{
    /// <summary>
    /// Max Age.
    /// The maximum age.
    /// </summary>
    public virtual TimeSpan? MaxAge { get; set; }

    /// <summary>
    /// Use Preload.
    /// Adds the preload directive, defaults to false.
    /// Max-age must be at least 18 weeks, and includeSubdomains must be enabled to use the preload directive.
    /// </summary>
    [Required]
    public virtual bool UsePreload { get; set; } = false;

    /// <summary>
    /// Include Subdomains.
    /// Adds includeSubDomains in the header, defaults to false
    /// </summary>
    [Required]
    public virtual bool IncludeSubdomains { get; set; } = false;
}