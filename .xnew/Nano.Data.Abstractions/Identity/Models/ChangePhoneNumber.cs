using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <inheritdoc />
public class ChangePhoneNumber : ChangePhoneNumber<Guid>;

/// <summary>
/// Change Phone Number.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class ChangePhoneNumber<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Token.
    /// </summary>
    [Required]
    public virtual string Token { get; set; }

    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }
}