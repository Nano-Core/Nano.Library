using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class ChangePhoneNumber : ChangePhoneNumber<Guid>;

/// <summary>
/// Represents a request to change a user's phone number.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class ChangePhoneNumber<TIdentity>
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
}