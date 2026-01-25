using System.ComponentModel.DataAnnotations;
using Nano.App.Config;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for JWT authentication.
/// </summary>
public class JwtAuthenticationOptions
{
    /// <summary>
    /// JWT issuer.
    /// </summary>
    [Required]
    public virtual string Issuer { get; set; } = "issuer";

    /// <summary>
    /// JWT audience.
    /// </summary>
    [Required]
    public virtual string Audience { get; set; } = "audience";

    /// <summary>
    /// Base64-encoded public key.
    /// </summary>
    [Required]
    public virtual string PublicKey { get; set; } = null!;

    /// <summary>
    /// Base64-encoded private key.
    /// </summary>
    [Required]
    public virtual string PrivateKey { get; set; } = null!;

    /// <summary>
    /// Expiration in minutes for the access token.
    /// </summary>
    [Required]
    public virtual int ExpirationInMinutes { get; set; } = 60;

    /// <summary>
    /// Expiration in hours for the refresh token.
    /// </summary>
    [Required]
    public virtual int RefreshExpirationInHours { get; set; } = 72;

    /// <summary>
    /// Optional root login options.
    /// </summary>
    public virtual LogInRootOptions? RootLogin { get; set; }

    /// <summary>
    /// External login options.
    /// </summary>
    [Required]
    public virtual ExternalLoginOptions ExternalLogins { get; set; } = new();
}