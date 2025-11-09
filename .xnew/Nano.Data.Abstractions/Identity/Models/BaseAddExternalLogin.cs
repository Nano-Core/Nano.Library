using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// Base Add External Login (abstract).
/// </summary>
/// <typeparam name="TProvider">The provider type.</typeparam>
/// <typeparam name="TIdentity">The identity type.</typeparam>
public abstract class BaseAddExternalLogin<TProvider, TIdentity>
    where TProvider : BaseLogInExternalProvider, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// Provider.
    /// </summary>
    public TProvider Provider { get; set; } = new();
}