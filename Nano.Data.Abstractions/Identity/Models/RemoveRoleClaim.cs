using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class RemoveRoleClaim : RemoveRoleClaim<Guid>;

/// <summary>
/// Represents a request to remove a claim from a role.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class RemoveRoleClaim<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the role.
    /// </summary>
    [Required]
    public virtual TIdentity RoleId { get; set; } = default!;

    /// <summary>
    /// The claim type to remove.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string ClaimType { get; set; } = null!;
}