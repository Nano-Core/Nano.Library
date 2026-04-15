using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External login request using externally supplied user data.
/// </summary>
public class LogInExternal : BaseLogIn
{
    /// <summary>
    /// The external user data used to complete the login.
    /// </summary>
    [Required]
    public virtual required ExternalAuthenticationData ExternalAuthenticationData { get; set; }
}

/// <summary>
/// Class for external login requests.
/// </summary>
/// <typeparam name="TFlow">The type of authentication flow, e.g. auth-code, implicit, etc.</typeparam>
public class LogInExternal<TFlow> : BaseLogIn
    where TFlow : BaseAuthFlow
{
    /// <summary>
    /// The external authentication flow.
    /// </summary>
    [Required]
    public virtual required TFlow Flow { get; set; }
}