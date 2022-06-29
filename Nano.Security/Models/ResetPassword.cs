using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// Reset Password.
/// </summary>
public class ResetPassword
{
    /// <summary>
    /// Token.
    /// </summary>
    [Required]
    public virtual string Token { get; set; }

    /// <summary>
    /// Email Address.
    /// </summary>
    [Required]
    [EmailAddress]
    public virtual string EmailAddress { get; set; }

    /// <summary>
    /// Password.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Password { get; set; }
}