using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Represents a username and password login request.
/// </summary>
public class LogIn : BaseLogIn
{
    /// <summary>
    /// The username used for authentication.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string Username { get; set; }

    /// <summary>
    /// The password used for authentication.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string Password { get; set; }
}