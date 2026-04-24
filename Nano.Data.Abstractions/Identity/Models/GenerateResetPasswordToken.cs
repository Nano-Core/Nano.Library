using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to generate a reset password token for a user by username.
/// </summary>
public class GenerateResetPasswordToken
{
    /// <summary>
    /// The username of the user for whom the reset token will be generated.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string Username { get; set; }
}