using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for https.
/// </summary>
public class HttpsOptions
{
    /// <summary>
    /// List of ports for HTTPS.
    /// </summary>
    [Required]
    public virtual int[] Ports { get; set; } = [];

    /// <summary>
    /// Enforce HTTPS required for all requests.
    /// </summary>
    [Required]
    public virtual bool UseHttpsRequired { get; set; } = false;

    /// <summary>
    /// SSL certificate configuration.
    /// </summary>
    public virtual CertificateOptions Certificate { get; set; } = new();
}