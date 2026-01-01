using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Sign In.
/// </summary>
public class SignIn : BaseSignIn
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
}