using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Entities.Identity;

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
    public virtual TIdentity IdentityUserId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the associated identity user.
    /// </summary>
    public virtual IdentityUserEx<TIdentity> IdentityUser { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the API key.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the hashed value of the API key.
    /// </summary>
    [Required]
    public virtual string Hash { get; set; } = null!;

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