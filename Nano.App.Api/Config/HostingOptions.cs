using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for hosting configuration.
/// </summary>
public class HostingOptions
{
    /// <summary>
    /// Root route for the application endpoints.
    /// </summary>
    [Required]
    public virtual string Root { get; set; } = "api";

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
    /// Options for Http.
    /// </summary>
    [Required]
    public virtual HttpOptions Http { get; set; } = new();

    /// <summary>
    /// Options for Https.
    /// </summary>
    public virtual HttpsOptions? Https { get; set; }

    /// <summary>
    /// Multipart upload limits.
    /// </summary>
    public virtual MultipartLimitsOptions? MultipartLimits { get; set; }
}