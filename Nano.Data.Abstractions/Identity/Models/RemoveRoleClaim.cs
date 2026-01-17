using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class RemoveRoleClaim : RemoveRoleClaim<Guid>;

/// <summary>
/// Remove Role Claim.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class RemoveRoleClaim<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Role Id.
    /// </summary>
    [Required]
    public virtual TIdentity RoleId { get; set; } = default!;

    /// <summary>
    /// Claim Type.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string ClaimType { get; set; } = null!;
}