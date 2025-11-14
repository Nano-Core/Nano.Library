using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class ConfirmPhoneNumberToken : ConfirmPhoneNumberToken<Guid>;

/// <summary>
/// Confirm Phone Number Token.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class ConfirmPhoneNumberToken<TIdentity>
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
}