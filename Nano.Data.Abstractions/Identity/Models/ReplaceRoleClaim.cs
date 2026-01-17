using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class ReplaceRoleClaim : ReplaceRoleClaim<Guid>;

/// <summary>
/// Replace Role Claim.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class ReplaceRoleClaim<TIdentity>
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

    /// <summary>
    /// New Claim Value.
    /// </summary>
    [Required]
    public virtual string NewClaimValue { get; set; } = null!;
}