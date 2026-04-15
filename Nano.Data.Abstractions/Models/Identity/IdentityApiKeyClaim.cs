using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Models.Identity;

/// <summary>
/// Represents a claim associated with an API key.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity key.</typeparam>
public class IdentityApiKeyClaim<TIdentity> : BaseEntityIdentity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Gets or sets the identifier of the associated API key.
    /// </summary>
    [Required]
    public virtual required TIdentity ApiKeyId { get; set; }

    /// <summary>
    /// Gets or sets the associated API key.
    /// </summary>
    public virtual IdentityApiKey<TIdentity> ApiKey { get; set; } = null!;

    /// <summary>
    /// Gets or sets the type of the claim.
    /// </summary>
    public virtual required string ClaimType { get; set; }

    /// <summary>
    /// Gets or sets the value of the claim.
    /// </summary>
    public virtual string? ClaimValue { get; set; }
}