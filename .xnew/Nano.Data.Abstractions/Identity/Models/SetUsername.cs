using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

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
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// New Username.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string NewUsername { get; set; }
}