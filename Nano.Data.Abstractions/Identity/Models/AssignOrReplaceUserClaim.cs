using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class AddOrReplaceUserClaim : ReplaceUserClaim<Guid>;

/// <summary>
/// Represents a request to add or replace a claim on a user.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class AssignOrReplaceUserClaim<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user whose claim should be added or replaced.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The claim type to add or replace.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string ClaimType { get; set; } = null!;

    /// <summary>
    /// The claim value to add or replace.
    /// </summary>
    [Required]
    public virtual string ClaimValue { get; set; } = null!;
}