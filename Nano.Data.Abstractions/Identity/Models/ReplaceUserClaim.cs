using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class ReplaceUserClaim : ReplaceUserClaim<Guid>;

/// <summary>
/// Represents a request to replace a user's claim with a new value.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class ReplaceUserClaim<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user whose claim is being replaced.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The claim type to replace.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string ClaimType { get; set; } = null!;

    /// <summary>
    /// The new value for the claim.
    /// </summary>
    [Required]
    public virtual string NewClaimValue { get; set; } = null!;
}