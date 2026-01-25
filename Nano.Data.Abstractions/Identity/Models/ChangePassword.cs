using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class ChangePassword : ChangePassword<Guid>;

/// <summary>
/// Represents a request to change a user's password.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class ChangePassword<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user whose password is being changed.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The user's current password.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string OldPassword { get; set; } = null!;

    /// <summary>
    /// The user's new password.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string NewPassword { get; set; } = null!;

    /// <summary>
    /// Confirmation of the new password. Must match <see cref="NewPassword"/>.
    /// </summary>
    [Required]
    [MaxLength(256)]
    [Compare(nameof(NewPassword))]
    public virtual string ConfirmNewPassword { get; set; } = null!;
}