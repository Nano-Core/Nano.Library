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
    public virtual required string Issuer { get; set; }

    /// <summary>
    /// JWT audience.
    /// </summary>
    [Required]
    public virtual required string Audience { get; set; }

    /// <summary>
    /// Base64-encoded public key.
    /// </summary>
    [Required]
    public virtual required string PublicKey { get; set; }

    /// <summary>
    /// Base64-encoded private key.
    /// </summary>
    public virtual string? PrivateKey { get; set; }

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