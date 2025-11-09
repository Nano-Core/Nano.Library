using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

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
    public virtual TIdentity RoleId { get; set; }

    /// <summary>
    /// Claim Type.
    /// </summary>
    [Required]
    public virtual string ClaimType { get; set; }

    /// <summary>
    /// New Claim Value.
    /// </summary>
    [Required]
    public virtual string NewClaimValue { get; set; }
}