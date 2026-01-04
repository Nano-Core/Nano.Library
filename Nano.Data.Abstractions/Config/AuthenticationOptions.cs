namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Authentication Options.
/// </summary>
public class AuthenticationOptions
{
    /// <summary>
    /// Jwt Options.
    /// </summary>
    public virtual JwtAuthenticationOptions Jwt { get; set; }

    /// <summary>
    /// Api Key.
    /// </summary>
    public virtual ApiKeyAuthenticationOptions ApiKey { get; set; }
}