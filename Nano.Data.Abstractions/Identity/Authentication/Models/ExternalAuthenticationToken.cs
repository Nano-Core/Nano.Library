using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External login token returned by an authentication provider.
/// </summary>
public class ExternalAuthenticationToken
{
    /// <summary>
    /// The name of the external authentication provider.
    /// </summary>
    [Required]
    public virtual required string Name { get; set; }

    /// <summary>
    /// The access token issued by the external provider.
    /// </summary>
    [Required]
    public virtual required string Token { get; set; }

    /// <summary>
    /// The refresh token issued by the external provider, if supported.
    /// </summary>
    public virtual string? RefreshToken { get; set; }
}