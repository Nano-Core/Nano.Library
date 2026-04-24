using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to sign in with username and password.
/// </summary>
public class SignIn
{
    /// <summary>
    /// The username of the user signing in.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string Username { get; set; }

    /// <summary>
    /// The password of the user signing in.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string Password { get; set; }
}