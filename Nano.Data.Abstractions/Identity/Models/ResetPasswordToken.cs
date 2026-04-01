using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to reset a user's password using a token.
/// </summary>
public class ResetPasswordToken
{
    /// <summary>
    /// The token used to authorize the password reset.
    /// </summary>
    [Required]
    public virtual string Token { get; set; } = null!;
}