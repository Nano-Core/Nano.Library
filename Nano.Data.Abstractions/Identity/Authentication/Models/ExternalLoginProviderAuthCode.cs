using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External Login Provider Auth Code.
/// </summary>
public class ExternalLoginProviderAuthCode : BaseLogInExternalProvider
{
    /// <summary>
    /// Code.
    /// </summary>
    [Required]
    public virtual string Code { get; set; } = null!;

    /// <summary>
    /// Code Verifier.
    /// </summary>
    [Required]
    public virtual string CodeVerifier { get; set; } = null!;

    /// <summary>
    /// Redirect Uri.
    /// </summary>
    [Required]
    public virtual string RedirectUri { get; set; } = null!;
}