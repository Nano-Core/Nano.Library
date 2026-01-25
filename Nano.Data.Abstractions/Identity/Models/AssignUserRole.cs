using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class AssignUserRole : AssignUserRole<Guid>;

/// <summary>
/// Represents a request to assign a role to a user.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class AssignUserRole<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user to assign the role to.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The name of the role to assign.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string RoleName { get; set; } = null!;
}