using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class RemoveExternalLogin : RemoveExternalLogin<Guid>;

/// <summary>
/// Represents a request to remove an external login from a user account.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class RemoveExternalLogin<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The external provider login to remove.
    /// </summary>
    [Required]
    public virtual ExternalProvider ExternalProvider { get; set; } = new();
}