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