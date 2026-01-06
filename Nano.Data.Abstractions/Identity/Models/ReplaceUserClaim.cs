using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class ReplaceUserClaim : ReplaceUserClaim<Guid>;

/// <summary>
/// Replace User Claim.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class ReplaceUserClaim<TIdentity>
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
    /// New Claim Value.
    /// </summary>
    [Required]
    public virtual string NewClaimValue { get; set; }
}