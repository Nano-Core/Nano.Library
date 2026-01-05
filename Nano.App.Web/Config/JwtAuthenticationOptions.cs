using Nano.App.Config;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.Web.Config;

/// <summary>
/// Jwt Options (nested class)
/// </summary>
public class JwtAuthenticationOptions
{
    /// <summary>
    /// Issuer.
    /// </summary>
    [Required]
    public virtual string Issuer { get; set; } = "issuer";

    /// <summary>
    /// Audience.
    /// </summary>
    [Required]
    public virtual string Audience { get; set; } = "audience";

    /// <summary>
    /// Public Key.
    /// Base64 encoded.
    /// </summary>
    [Required]
    public virtual string PublicKey { get; set; }

    /// <summary>
    /// Private Key.
    /// Base64 encoded.
    /// </summary>
    [Required]
    public virtual string PrivateKey { get; set; }

    /// <summary>
    /// Expiration In Minutes.
    /// </summary>
    [Required]
    public virtual int ExpirationInMinutes { get; set; } = 60;

    /// <summary>
    /// Refresh Expiration In Hours.
    /// </summary>
    [Required]
    public virtual int RefreshExpirationInHours { get; set; } = 72;

    /// <summary>
    /// Log In Root Options.
    /// </summary>
    public virtual LogInRootOptions RootLogin { get; set; }

    /// <summary>
    /// External Logins.
    /// </summary>
    [Required]
    public virtual ExternalLoginOptions ExternalLogins { get; set; } = new();
}