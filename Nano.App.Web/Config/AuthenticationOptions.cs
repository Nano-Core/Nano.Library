using Nano.App.Config;

namespace Nano.App.Web.Config;

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
    /// Log In Root Options.
    /// </summary>
    public virtual LogInRootOptions RootLogin { get; set; }
}