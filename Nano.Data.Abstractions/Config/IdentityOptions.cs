using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Identity Options.
/// </summary>
public class IdentityOptions
{
    /// <summary>
    /// Tokens Expiration.
    /// </summary>
    [Required]
    public virtual int TokensExpirationInHours { get; set; } = 24;

    /// <summary>
    /// User.
    /// </summary>
    [Required]
    public virtual UserOptions User { get; set; } = new();

    /// <summary>
    /// Sign In.
    /// </summary>
    [Required]
    public virtual SignInOptions SignIn { get; set; } = new();

    /// <summary>
    /// Lockout.
    /// </summary>
    [Required]
    public virtual LockoutOptions Lockout { get; set; } = new();

    /// <summary>
    /// Password.
    /// </summary>
    [Required]
    public virtual PasswordOptions Password { get; set; } = new();

    /// <summary>
    /// Authentication.
    /// </summary>
    [Required]
    public virtual AuthenticationOptions Authentication { get; set; } = new();
}