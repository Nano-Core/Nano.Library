using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Models.Identity;

/// <summary>
/// Represents an API key associated with an identity user.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity key.</typeparam>
public class IdentityApiKey<TIdentity> : BaseEntityIdentity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Gets or sets the identity user Id.
    /// </summary>
    [Required]
    public virtual required TIdentity IdentityUserId { get; set; }

    /// <summary>
    /// Gets or sets the associated identity user.
    /// </summary>
    public virtual IdentityUserEx<TIdentity> IdentityUser { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the API key.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string Name { get; set; }

    /// <summary>
    /// Gets or sets the hashed value of the API key.
    /// </summary>
    [Required]
    public virtual required string Hash { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the API key was created.
    /// </summary>
    [Required]
    public virtual DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the timestamp when the API key was revoked, if applicable.
    /// </summary>
    public virtual DateTimeOffset? RevokedAt { get; set; }
}