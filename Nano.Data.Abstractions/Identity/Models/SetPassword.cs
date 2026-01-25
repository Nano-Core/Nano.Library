using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class SetPassword : SetPassword<Guid>;

/// <summary>
/// Represents a request to set a new password for a user.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class SetPassword<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user whose password is being set.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The new password to assign to the user.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string NewPassword { get; set; } = null!;
}