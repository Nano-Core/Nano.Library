using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class RemoveUserRole : RemoveUserRole<Guid>;

/// <summary>
/// Represents a request to remove a role from a user.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class RemoveUserRole<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The name of the role to remove.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string RoleName { get; set; } = null!;
}