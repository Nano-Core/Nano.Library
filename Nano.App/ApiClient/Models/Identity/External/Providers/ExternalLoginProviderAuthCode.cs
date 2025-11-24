using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Models.Identity.External.Providers;

/// <summary>
/// External Login Provider Auth Code.
/// </summary>
public class ExternalLoginProviderAuthCode : BaseLogInExternalProvider
{
    /// <summary>
    /// Code.
    /// </summary>
    [Required]
    public virtual string Code { get; set; }

    /// <summary>
    /// Code Verifier.
    /// </summary>
    [Required]
    public virtual string CodeVerifier { get; set; }

    /// <summary>
    /// Redirect Uri.
    /// </summary>
    [Required]
    public virtual string RedirectUri { get; set; }
}