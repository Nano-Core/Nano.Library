using Nano.Common.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class ChangePhoneNumberToken : ChangePhoneNumberToken<Guid>;

/// <summary>
/// Represents a request to change a user's phone number using a confirmation token.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class ChangePhoneNumberToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user whose phone number is being changed.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The token used to authorize the phone number change.
    /// </summary>
    [Required]
    public virtual string Token { get; set; } = null!;

    /// <summary>
    /// The new phone number to assign to the user.
    /// </summary>
    [Required]
    [MaxLength(20)]
    [InternationalPhone]
    public virtual string NewPhoneNumber { get; set; } = null!;
}