using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// 
/// </summary>
public class LogInRoot
{
    /// <summary>
    /// Username.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public virtual string Username { get; set; }

    /// <summary>
    /// Password.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public virtual string Password { get; set; }
}