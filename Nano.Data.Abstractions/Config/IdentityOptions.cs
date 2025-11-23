namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Identity Options.
/// </summary>
public class IdentityOptions
{
    /// <summary>
    /// Tokens Expiration.
    /// </summary>
    public virtual int TokensExpirationInHours { get; set; } = 24;

    /// <summary>
    /// User Options.
    /// </summary>
    public virtual UserOptions User { get; set; } = new();

    /// <summary>
    /// Sign In Options.
    /// </summary>
    public virtual SignInOptions SignIn { get; set; } = new();

    /// <summary>
    /// Lockout Options.
    /// </summary>
    public virtual LockoutOptions Lockout { get; set; } = new();

    /// <summary>
    /// Password Options.
    /// </summary>
    public virtual PasswordOptions Password { get; set; } = new();
}