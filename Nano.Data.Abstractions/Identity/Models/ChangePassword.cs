using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to change a user's password.
/// </summary>
public class ChangePassword
{
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