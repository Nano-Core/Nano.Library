using System.ComponentModel.DataAnnotations;
using Nano.App.Web.Config.Enums;
using NWebsec.AspNetCore.Mvc;

namespace Nano.App.Web.Config;

/// <summary>
/// Http Policy Header Options.
/// </summary>
public class HttpPolicyHeaderOptions
{
    /// <summary>
    /// Use Content Type Options.
    /// Added X-Content-Type Options header.
    /// </summary>
    [Required]
    public virtual bool UseContentTypeOptions { get; set; } = true;

    /// <summary>
    /// Use Referrer Policy Header.
    /// </summary>
    [Required]
    public virtual ReferrerPolicy ReferrerPolicyHeader { get; set; } = ReferrerPolicy.Disabled;

    /// <summary>
    /// Use Frame Options Policy Header.
    /// </summary>
    [Required]
    public virtual XFrameOptionsPolicy FrameOptionsPolicyHeader { get; set; } = XFrameOptionsPolicy.Disabled;

    /// <summary>
    /// Use Xss Protection Policy Header.
    /// </summary>
    [Required]
    public virtual XXssProtectionPolicyBlockMode XssProtectionPolicyHeader { get; set; } = XXssProtectionPolicyBlockMode.Disabled;

    /// <summary>
    /// Csp.
    /// Settings for Content-Security-Policy.
    /// </summary>
    public virtual CspOptions Csp { get; set; }

    /// <summary>
    /// Cors.
    /// </summary>
    public virtual CorsOptions Cors { get; set; }

    /// <summary>
    /// Hsts.
    /// Settings for Strict-Transport-Security.
    /// </summary>
    public virtual HstsOptions Hsts { get; set; }

    /// <summary>
    /// Robots.
    /// Settings for robots (search engines) behavior.
    /// </summary>
    public virtual RobotOptions Robots { get; set; }
}