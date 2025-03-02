using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

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
    public virtual TIdentity RoleId { get; set; }

    /// <summary>
    /// Claim Type.
    /// </summary>
    [Required]
    public virtual string ClaimType { get; set; }
}