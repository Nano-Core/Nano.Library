using System.ComponentModel.DataAnnotations;

namespace Nano.App.Config;

/// <summary>
/// Credentials used to enable a transient root-level login.
/// </summary>
public class LogInRootOptions
{
    /// <summary>
    /// The username used for root authentication.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string Username { get; set; }

    /// <summary>
    /// The password used for root authentication.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string Password { get; set; }
}