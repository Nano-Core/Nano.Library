using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class ChangeEmailToken : ChangeEmailToken<Guid>;

/// <summary>
/// Change Email Token
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class ChangeEmailToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// Token.
    /// </summary>
    [Required]
    public virtual string Token { get; set; } = null!;

    /// <summary>
    /// New Email Address.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public virtual string NewEmailAddress { get; set; } = null!;
}