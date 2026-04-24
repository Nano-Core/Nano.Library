namespace Nano.App.Api.Config;

/// <summary>
/// Options for HTTP policy headers.
/// </summary>
public class HttpPolicyHeaderOptions
{
    /// <summary>
    /// Content-Type header options.
    /// Prevents attacks like MIME type confusion, which can lead to XSS or other client-side vulnerabilities.
    /// </summary>
    public virtual ContentTypeOptions? ContentType { get; set; }

    /// <summary>
    /// Referrer-Policy header options.
    /// </summary>
    public virtual ReferrerPolicyOptions? ReferrerPolicy { get; set; }

    /// <summary>
    /// X-Frame-Options header options.
    /// </summary>
    public virtual FrameOptionsOptions? FrameOptions { get; set; }

    /// <summary>
    /// XSS-Protection header options.
    /// </summary>
    public virtual XssProtectionOptions? XssProtection { get; set; }

    /// <summary>
    /// Content-Security-Policy options.
    /// </summary>
    public virtual CspOptions? Csp { get; set; }

    /// <summary>
    /// CORS configuration options.
    /// </summary>
    public virtual CorsOptions? Cors { get; set; }

    /// <summary>
    /// HSTS configuration options.
    /// </summary>
    public virtual HstsOptions? Hsts { get; set; }

    /// <summary>
    /// Robots meta tag options.
    /// </summary>
    public virtual RobotsOptions? Robots { get; set; }

    /// <summary>
    /// Forwarded headers configuration.
    /// </summary>
    public virtual ForwardedHeadersOptions? ForwardedHeaders { get; set; }
}