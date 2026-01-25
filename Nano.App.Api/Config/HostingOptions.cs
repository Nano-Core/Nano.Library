using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for hosting configuration.
/// </summary>
public class HostingOptions
{
    /// <summary>
    /// Root path for the application.
    /// </summary>
    [Required]
    public virtual string Root { get; set; } = "api";

    /// <summary>
    /// List of ports for HTTP.
    /// </summary>
    [Required]
    public virtual int[] Ports { get; set; } = [];

    /// <summary>
    /// List of ports for HTTPS.
    /// </summary>
    [Required]
    public virtual int[] PortsHttps { get; set; } = [];

    /// <summary>
    /// Enforce HTTPS redirect for all requests.
    /// </summary>
    [Required]
    public virtual bool UseHttpsRequired { get; set; } = false;

    /// <summary>
    /// Expose detailed errors.
    /// </summary>
    [Required]
    public virtual bool ExposeErrors { get; set; } = false;

    /// <summary>
    /// Expose authentication controller.
    /// </summary>
    [Required]
    public virtual bool ExposeAuthController { get; set; } = true;

    /// <summary>
    /// Expose audit controller.
    /// </summary>
    [Required]
    public virtual bool ExposeAuditController { get; set; } = true;

    /// <summary>
    /// SSL certificate configuration.
    /// </summary>
    public virtual CertificateOptions? Certificate { get; set; }

    /// <summary>
    /// Multipart upload limits.
    /// </summary>
    public virtual MultipartLimitsOptions? MultipartLimits { get; set; }
}