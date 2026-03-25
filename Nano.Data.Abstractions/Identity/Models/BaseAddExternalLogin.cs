using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Base type for requests that add an external login to an existing user.
/// </summary>
/// <typeparam name="TProvider">The external authentication provider type.</typeparam>
/// <typeparam name="TFlow">The type of authentication flow, e.g. auth-code, implicit, etc.</typeparam>
public abstract class BaseAddExternalLogin<TProvider, TFlow>
    where TProvider : BaseExternalProvider, new()
    where TFlow : BaseAuthFlow, new()
{
    /// <summary>
    /// The external authentication provider configuration.
    /// </summary>
    [Required]
    public TProvider Provider { get; set; } = new();

    /// <summary>
    /// The flow used for authentication.
    /// </summary>
    public virtual TFlow Flow { get; set; } = new();
}