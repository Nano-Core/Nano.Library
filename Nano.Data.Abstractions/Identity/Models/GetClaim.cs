using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class GetClaim : GetClaim<Guid>;

/// <summary>
/// Represents a request to get a specific claim for a user.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class GetClaim<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The type of claim to retrieve.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string ClaimType { get; set; } = null!;
}