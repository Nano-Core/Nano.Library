using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class AddOrReplaceUserClaim : ReplaceUserClaim<Guid>;

/// <summary>
/// Add Or Replace User Claim.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class AssignOrReplaceClaim<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }

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