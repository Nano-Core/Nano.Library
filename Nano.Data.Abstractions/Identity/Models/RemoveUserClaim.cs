using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class RemoveUserClaim : RemoveUserClaim<Guid>;

/// <summary>
/// Represents a request to remove a claim from a user.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class RemoveUserClaim<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The claim type to remove.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string ClaimType { get; set; } = null!;
}