using System;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Annotations;

namespace Nano.Data.Abstractions.Models.Identity;

/// <summary>
/// Represents a newly created API key for an identity user.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity key.</typeparam>
public class IdentityApiKeyCreated<TIdentity> : IdentityApiKey<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Gets or sets the associated identity API key entity.
    /// </summary>
    [Required]
    [Include]
    public virtual IdentityApiKey<TIdentity> IdentityApiKey { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unencrypted hash of the API key.
    /// </summary>
    [Required]
    public virtual string UnencryptedHash { get; set; } = null!;
}