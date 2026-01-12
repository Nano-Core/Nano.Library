namespace Nano.App.Api.Config;

/// <summary>
/// Authentication Options.
/// </summary>
public class AuthenticationOptions
{
    /// <summary>
    /// Jwt Options.
    /// </summary>
    public virtual JwtAuthenticationOptions Jwt { get; set; }
}