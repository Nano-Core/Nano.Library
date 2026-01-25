using System;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Base type for requests that add an external login to an existing user.
/// </summary>
/// <typeparam name="TProvider">The external authentication provider type.</typeparam>
/// <typeparam name="TIdentity">The user identity type.</typeparam>
public abstract class BaseAddExternalLogin<TProvider, TIdentity>
    where TProvider : BaseLogInExternalProvider, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user to associate with the external login.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The external authentication provider configuration.
    /// </summary>
    [Required]
    public TProvider Provider { get; set; } = new();
}