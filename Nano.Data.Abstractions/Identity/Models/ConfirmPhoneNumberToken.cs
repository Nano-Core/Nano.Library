using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class ConfirmPhoneNumberToken : ConfirmPhoneNumberToken<Guid>;

/// <summary>
/// Represents a request to confirm a user's phone number using a token.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class ConfirmPhoneNumberToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user whose phone number is being confirmed.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The token used to confirm the user's phone number.
    /// </summary>
    [Required]
    public virtual string Token { get; set; } = null!;
}