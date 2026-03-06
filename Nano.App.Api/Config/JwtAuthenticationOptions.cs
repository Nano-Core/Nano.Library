using System;
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
    public virtual string Issuer { get; set; } = null!;

    /// <summary>
    /// JWT audience.
    /// </summary>
    [Required]
    public virtual string Audience { get; set; } = null!;

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
    /// Expiration for the access token.
    /// </summary>
    [Required]
    public virtual TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(60);

    /// <summary>
    /// Expiration for the refresh token.
    /// </summary>
    [Required]
    public virtual TimeSpan RefreshExpiration { get; set; } = TimeSpan.FromHours(72);

    /// <summary>
    /// Optional root login options.
    /// </summary>
    public virtual LogInRootOptions? RootLogin { get; set; }

    /// <summary>
    /// Optional external login options.
    /// </summary>
    [Required]
    public virtual ExternalLoginOptions ExternalLogins { get; set; } = new();
}