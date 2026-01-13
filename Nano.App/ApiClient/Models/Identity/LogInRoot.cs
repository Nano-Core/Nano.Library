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
    [MaxLength(256)]
    public virtual string Username { get; set; } = null!;

    /// <summary>
    /// Password.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Password { get; set; } = null!;
}