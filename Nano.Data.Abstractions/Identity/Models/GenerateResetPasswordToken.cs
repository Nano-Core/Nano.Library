using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Generate Reset Password Token.
/// </summary>
public class GenerateResetPasswordToken
{
    /// <summary>
    /// Username.
    /// </summary>
    [Required]
    public virtual string Username { get; set; } = null!;
}