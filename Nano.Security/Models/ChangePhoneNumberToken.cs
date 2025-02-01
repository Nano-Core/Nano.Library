using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <inheritdoc />
public class ChangePhoneNumberToken : ChangePhoneNumberToken<Guid>;

/// <summary>
/// Change Phone Number Token
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class ChangePhoneNumberToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// Token.
    /// </summary>
    [Required]
    public virtual string Token { get; set; }

    /// <summary>
    /// New Phone Number.
    /// </summary>
    [Required]
    public virtual string NewPhoneNumber { get; set; }
}