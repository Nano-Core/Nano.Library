using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class ReplaceRoleClaim : ReplaceRoleClaim<Guid>;

/// <summary>
/// Represents a request to replace a role's claim with a new value.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class ReplaceRoleClaim<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the role whose claim is being replaced.
    /// </summary>
    [Required]
    public virtual TIdentity RoleId { get; set; } = default!;

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