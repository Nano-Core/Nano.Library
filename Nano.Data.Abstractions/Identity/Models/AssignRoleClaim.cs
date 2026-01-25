using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class AssignRoleClaim : AssignRoleClaim<Guid>;

/// <summary>
/// Represents a request to assign a claim to a role.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class AssignRoleClaim<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the role to assign the claim to.
    /// </summary>
    [Required]
    public virtual TIdentity RoleId { get; set; } = default!;

    /// <summary>
    /// The claim type to assign.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string ClaimType { get; set; } = null!;

    /// <summary>
    /// The claim value to assign.
    /// </summary>
    [Required]
    public virtual string ClaimValue { get; set; } = null!;
}