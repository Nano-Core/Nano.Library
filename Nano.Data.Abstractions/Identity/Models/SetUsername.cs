using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class SetUsername : SetUsername<Guid>;

/// <summary>
/// Represents a request to update a user's username.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class SetUsername<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user whose username is being updated.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The new username to assign to the user.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string NewUsername { get; set; } = null!;
}