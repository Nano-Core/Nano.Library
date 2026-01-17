namespace Nano.App.Api.Config;

/// <summary>
/// Http Policy Header Options.
/// </summary>
public class HttpPolicyHeaderOptions
{
    /// <summary>
    /// 
    /// </summary>
    public virtual ContentTypeOptions? ContentType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ReferrerPolicyOptions? ReferrerPolicy { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual XFrameOptionsOptions? XFrameOptions { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual XXssProtectionOptions? XXssProtection { get; set; }

    /// <summary>
    /// Csp.
    /// Settings for Content-Security-Policy.
    /// </summary>
    public virtual CspOptions? Csp { get; set; }

    /// <summary>
    /// Cors.
    /// </summary>
    public virtual CorsOptions? Cors { get; set; }

    /// <summary>
    /// Hsts.
    /// Settings for Strict-Transport-Security.
    /// </summary>
    public virtual HstsOptions? Hsts { get; set; }

    /// <summary>
    /// Robots.
    /// Settings for robots (search engines) behavior.
    /// </summary>
    public virtual RobotsOptions? Robots { get; set; }

    /// <summary>
    /// Enables forwarded headers, when application is behind a proxy.
    /// </summary>
    public virtual ForwardedHeadersOptions? ForwardedHeaders { get; set; }
}