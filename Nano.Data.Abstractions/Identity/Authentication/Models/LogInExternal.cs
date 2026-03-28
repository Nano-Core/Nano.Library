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
    public virtual ExternalAuthenticationData ExternalAuthenticationData { get; set; } = new();
}

/// <summary>
/// Base class for external login requests.
/// </summary>
/// <typeparam name="TFlow">The type of authentication flow, e.g. auth-code, implicit, etc.</typeparam>
public abstract class LogInExternal<TFlow> : BaseLogIn
    where TFlow : BaseAuthFlow
{
    /// <summary>
    /// The external authentication flow.
    /// </summary>
    [Required]
    public virtual TFlow Flow { get; set; } = null!;
}