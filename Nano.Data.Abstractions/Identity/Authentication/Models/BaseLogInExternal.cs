using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Base class for external login requests.
/// </summary>
/// <typeparam name="TProvider">The external provider type.</typeparam>
/// <typeparam name="TFlow">The type of authentication flow, e.g. auth-code, implicit, etc.</typeparam>
public abstract class BaseLogInExternal<TProvider, TFlow> : LogInExternal
    where TProvider : BaseExternalProvider, new()
    where TFlow : BaseAuthFlow, new()
{
    /// <summary>
    /// The external authentication provider.
    /// </summary>
    [Required]
    public virtual TProvider Provider { get; set; } = new();

    /// <summary>
    /// The flow used for authentication.
    /// </summary>
    [Required]
    public virtual TFlow Flow { get; set; } = new();
}