using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// Reset Password.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class ResetPassword<TIdentity>
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
    /// Password.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Password { get; set; }
}