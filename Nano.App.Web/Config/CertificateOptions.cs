using System.ComponentModel.DataAnnotations;

namespace Nano.App.Web.Config;

/// <summary>
/// Certificate Options.
/// </summary>
public class CertificateOptions
{
    /// <summary>
    /// Path.
    /// </summary>
    [Required]
    public virtual string Path { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    [Required]
    public virtual string Password { get; set; }
}