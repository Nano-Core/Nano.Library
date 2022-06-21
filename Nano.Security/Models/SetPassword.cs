using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// Set Password.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class SetPassword<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// New Password.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string NewPassword { get; set; }
}