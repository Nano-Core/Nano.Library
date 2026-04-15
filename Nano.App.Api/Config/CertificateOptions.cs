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
    public virtual required string Path { get; set; }

    /// <summary>
    /// Password for the certificate.
    /// </summary>
    [Required]
    public virtual required string Password { get; set; }
}