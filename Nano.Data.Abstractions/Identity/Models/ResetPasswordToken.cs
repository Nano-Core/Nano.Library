using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to reset a user's password using a token.
/// </summary>
public class ResetPasswordToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The id of the user.
    /// </summary>
    [Required]
    public virtual required TIdentity Id { get; set; }

    /// <summary>
    /// The token used to authorize the password reset.
    /// </summary>
    [Required]
    public virtual required string Token { get; set; }
}