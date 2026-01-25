using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for SSL certificates.
/// </summary>
public class CertificateOptions
{
    /// <summary>
    /// File path to the certificate.
    /// </summary>
    [Required]
    public virtual string Path { get; set; } = null!;

    /// <summary>
    /// Password for the certificate.
    /// </summary>
    [Required]
    public virtual string Password { get; set; } = null!;
}