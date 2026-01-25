using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Aggregates all identity-related configuration options, including users,
/// sign-in requirements, password policies, lockout settings, and authentication.
/// </summary>
public class IdentityOptions
{
    /// <summary>
    /// Gets or sets the expiration time for tokens in hours.
    /// Defaults to <c>24</c>.
    /// </summary>
    [Required]
    public virtual int TokensExpirationInHours { get; set; } = 24;

    /// <summary>
    /// Gets or sets the user-related options, such as uniqueness requirements and default roles.
    /// </summary>
    [Required]
    public virtual UserOptions User { get; set; } = new();

    /// <summary>
    /// Gets or sets the sign-in options, such as requiring confirmed email or phone number.
    /// </summary>
    [Required]
    public virtual SignInOptions SignIn { get; set; } = new();

    /// <summary>
    /// Gets or sets the account lockout options.
    /// </summary>
    [Required]
    public virtual LockoutOptions Lockout { get; set; } = new();

    /// <summary>
    /// Gets or sets the password policy options.
    /// </summary>
    [Required]
    public virtual PasswordOptions Password { get; set; } = new();

    /// <summary>
    /// Gets or sets the authentication options, such as API key configuration.
    /// </summary>
    [Required]
    public virtual AuthenticationOptions Authentication { get; set; } = new();
}