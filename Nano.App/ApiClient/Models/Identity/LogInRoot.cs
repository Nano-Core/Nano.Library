using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Models.Identity;

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