using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Nano.Data.Abstractions.Models.Identity;

/// <summary>
/// Represents a relationship between an API key and a role.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity key.</typeparam>
public class IdentityApiKeyRole<TIdentity> : BaseEntityIdentity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Gets or sets the identifier of the associated API key.
    /// </summary>
    [Required]
    public virtual TIdentity ApiKeyId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the associated API key.
    /// </summary>
    public virtual IdentityApiKey<TIdentity> ApiKey { get; set; } = null!;

    /// <summary>
    /// Gets or sets the identifier of the associated role.
    /// </summary>
    [Required]
    public virtual TIdentity RoleId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the associated role.
    /// </summary>
    public virtual IdentityRole<TIdentity> Role { get; set; } = null!;
}