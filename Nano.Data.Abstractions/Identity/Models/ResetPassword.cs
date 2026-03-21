using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to reset a user's password.
/// </summary>
public class ResetPassword
{
    /// <summary>
    /// The token used to authorize the password reset.
    /// </summary>
    [Required]
    public virtual string Token { get; set; } = null!;

    /// <summary>
    /// The new password to assign to the user.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Password { get; set; } = null!;
}