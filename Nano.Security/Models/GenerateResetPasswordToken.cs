using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// Generate Reset Password Token.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class GenerateResetPasswordToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }
}