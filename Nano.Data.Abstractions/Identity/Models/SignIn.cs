using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Sign In.
/// </summary>
public class SignIn
{
    /// <summary>
    /// Username.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Username { get; set; }

    /// <summary>
    /// Password.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Password { get; set; }

    /// <summary>
    /// Is Remember Me.
    /// </summary>
    [Required]
    [DefaultValue(false)]
    public virtual bool IsRememberMe { get; set; } = false;
}