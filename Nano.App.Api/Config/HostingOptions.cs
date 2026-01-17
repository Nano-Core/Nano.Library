using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Hosting Options.
/// </summary>
public class HostingOptions
{
    /// <summary>
    /// Root.
    /// </summary>
    [Required]
    public virtual string Root { get; set; } = "api";

    /// <summary>
    /// Ports.
    /// </summary>
    [Required]
    public virtual int[] Ports { get; set; } = [];

    /// <summary>
    /// Ports Https.
    /// </summary>
    [Required]
    public virtual int[] PortsHttps { get; set; } = [];

    /// <summary>
    /// Use Https Redirect.
    /// Forces https for all requests.
    /// </summary>
    [Required]
    public virtual bool UseHttpsRequired { get; set; } = false;

    /// <summary>
    /// Expose Errors.
    /// </summary>
    [Required]
    public virtual bool ExposeErrors { get; set; } = false;

    /// <summary>
    /// Expose Auth Controller.
    /// </summary>
    [Required]
    public virtual bool ExposeAuthController { get; set; } = true;

    /// <summary>
    /// Expose Audit Controller.
    /// </summary>
    [Required]
    public virtual bool ExposeAuditController { get; set; } = true;

    /// <summary>
    /// Certificate (ssl)
    /// </summary>
    public virtual CertificateOptions? Certificate { get; set; }

    /// <summary>
    /// Upload.
    /// </summary>
    public virtual MultipartLimitsOptions? MultipartLimits { get; set; }
}