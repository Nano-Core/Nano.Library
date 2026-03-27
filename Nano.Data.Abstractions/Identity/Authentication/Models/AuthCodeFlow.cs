using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Identity.Authentication.Models.Enums;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Auth code flow.
/// </summary>
public class AuthCodeFlow() : BaseAuthFlow(AuthFlowType.AuthCode)
{
    /// <summary>
    /// The authorization code returned by the external provider.
    /// </summary>
    [Required]
    public virtual string Code { get; set; } = null!;

    /// <summary>
    /// The PKCE code verifier associated with the authorization code.
    /// </summary>
    [Required]
    public virtual string CodeVerifier { get; set; } = null!;

    /// <summary>
    /// The redirect URI used during the authentication flow.
    /// </summary>
    [Required]
    public virtual string RedirectUri { get; set; } = null!;
}