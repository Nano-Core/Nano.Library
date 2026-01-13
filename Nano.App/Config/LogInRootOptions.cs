using System.ComponentModel.DataAnnotations;

namespace Nano.App.Config;

/// <summary>
/// Log In Root Options.
/// </summary>
public class LogInRootOptions
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