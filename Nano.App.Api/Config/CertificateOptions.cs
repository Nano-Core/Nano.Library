using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Certificate Options.
/// </summary>
public class CertificateOptions
{
    /// <summary>
    /// Path.
    /// </summary>
    [Required]
    public virtual string Path { get; set; } = null!;

    /// <summary>
    /// Password
    /// </summary>
    [Required]
    public virtual string Password { get; set; } = null!;
}