namespace Nano.Security;

/// <summary>
/// Jwt Options (nested class)
/// </summary>
public class JwtAuthenticationOptions
{
    /// <summary>
    /// Is Enabled.
    /// </summary>
    public virtual bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Issuer.
    /// </summary>
    public virtual string Issuer { get; set; } = "issuer";

    /// <summary>
    /// Audience.
    /// </summary>
    public virtual string Audience { get; set; } = "audience";

    // BUG: DATA: Default values is not valid, so make null and check it
    /// <summary>
    /// Public Key.
    /// Base64 encoded.
    /// </summary>
    public virtual string PublicKey { get; set; } = "publicsecretkey";

    /// <summary>
    /// Private Key.
    /// Base64 encoded.
    /// </summary>
    public virtual string PrivateKey { get; set; } = "veryprivatesecretkey";

    /// <summary>
    /// Expiration In Minutes.
    /// </summary>
    public virtual int ExpirationInMinutes { get; set; } = 60;

    /// <summary>
    /// Refresh Expiration In Hours.
    /// </summary>
    public virtual int RefreshExpirationInHours { get; set; } = 72;

    /// <summary>
    /// External Logins.
    /// </summary>
    public virtual ExternalLoginOptions ExternalLogins { get; set; } = new();
}