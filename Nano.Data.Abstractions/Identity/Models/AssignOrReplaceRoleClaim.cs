using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class AddOrReplaceRoleClaim : ReplaceUserClaim<Guid>;

/// <summary>
/// Add Or Replace Role Claim.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class AssignOrReplaceRoleClaim<TIdentity>
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
    /// Claim Value.
    /// </summary>
    [Required]
    public virtual string ClaimValue { get; set; }
}