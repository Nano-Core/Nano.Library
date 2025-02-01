using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <inheritdoc />
public class ChangePassword : ChangePassword<Guid>;

/// <summary>
/// Change Password.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class ChangePassword<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// Old Password.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string OldPassword { get; set; }

    /// <summary>
    /// New Password.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string NewPassword { get; set; }

    /// <summary>
    /// Confirm New Passowrd.
    /// </summary>
    [Required]
    [MaxLength(256)]
    [Compare("NewPassword")]
    public virtual string ConfirmNewPassword { get; set; }
}