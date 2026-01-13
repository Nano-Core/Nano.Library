using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class CreateApiKey : CreateApiKey<Guid>;

/// <summary>
/// Create Api Key.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class CreateApiKey<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// Name.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Expire At.
    /// </summary>
    public virtual DateTimeOffset? ExpireAt { get; set; }
}