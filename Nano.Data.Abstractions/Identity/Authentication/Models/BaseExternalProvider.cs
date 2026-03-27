using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

// BUG: NAME SHOULD NOT BE ON ENDPOINT REQUESTS. WE GET THE NAME FROM THE ROUTE, AND ON THE LoginExternalRuquest<TProvider, TFlow> we end up with an empty provider in request

// BUG: 000: Name: This is a problem for the Login external custm request, we need setter and parameterless constructor
// When having a generic request we need to use when creating dynamic endpoints might be a problem for this non-parameterless constructor.

/// <summary>
/// Base class for external providers.
/// </summary>
public abstract class BaseExternalProvider(string name)
{
    /// <summary>
    /// The unique name of the external provider.
    /// </summary>
    [Required]
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));
}

/// <summary>
/// Base class for external providers, defining the authentication flow.
/// </summary>
/// <typeparam name="TFlow">The type of authentication flow.</typeparam>
public abstract class BaseExternalProvider<TFlow>(string name) : BaseExternalProvider(name)
    where TFlow : BaseAuthFlow
{
    /// <summary>
    /// The authentication flow of the provider.
    /// </summary>
    public virtual TFlow Flow { get; set; } = null!;
}