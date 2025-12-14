using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class RemoveExternalLogin : RemoveExternalLogin<Guid>;

/// <summary>
/// Remove External Login.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class RemoveExternalLogin<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// External Provider.
    /// </summary>
    [Required]
    public virtual ExternalProvider ExternalProvider { get; set; } = new();
}