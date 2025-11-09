using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// Generate Reset Password Token.
/// </summary>
public class GenerateResetPasswordToken
{
    /// <summary>
    /// Username.
    /// </summary>
    [Required]
    public virtual string Username { get; set; }
}