using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class SetUsername : SetUsername<Guid>;

/// <summary>
/// Set Username.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class SetUsername<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// New Username.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string NewUsername { get; set; } = null!;
}