using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class ResetPasswordToken : ResetPasswordToken<Guid>;

/// <summary>
/// Represents a request to reset a user's password using a token.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class ResetPasswordToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user whose password is being reset.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The token used to authorize the password reset.
    /// </summary>
    [Required]
    public virtual string Token { get; set; } = null!;
}