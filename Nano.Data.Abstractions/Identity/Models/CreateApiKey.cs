using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class CreateApiKey : CreateApiKey<Guid>;

/// <summary>
/// Represents a request to create an API key for a user.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class CreateApiKey<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user for whom the API key is created.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The name of the API key.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// The optional expiration date of the API key.
    /// </summary>
    public virtual DateTimeOffset? ExpireAt { get; set; }
}