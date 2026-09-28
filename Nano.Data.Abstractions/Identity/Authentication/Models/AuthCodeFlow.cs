using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Auth code flow.
/// </summary>
public class AuthCodeFlow : BaseAuthFlow
{
    /// <summary>
    /// The authorization code returned by the external provider.
    /// </summary>
    [Required]
    public virtual required string Code { get; set; }

    /// <summary>
    /// The PKCE code verifier associated with the authorization code.
    /// </summary>
    [Required]
    public virtual required string CodeVerifier { get; set; }

    /// <summary>
    /// The redirect URI used during the authentication flow.
    /// </summary>
    [Required]
    public virtual required string RedirectUri { get; set; }
}
