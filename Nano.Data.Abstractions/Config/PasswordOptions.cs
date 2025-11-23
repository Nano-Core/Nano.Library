namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Password Options (nested class).
/// </summary>
public class PasswordOptions
{
    /// <summary>
    /// Require Digit.
    /// </summary>
    public virtual bool RequireDigit { get; set; } = false;

    /// <summary>
    /// Require Non Alphanumeric.
    /// </summary>
    public virtual bool RequireNonAlphanumeric { get; set; } = false;

    /// <summary>
    /// Require Lowercase.
    /// </summary>
    public virtual bool RequireLowercase { get; set; } = false;

    /// <summary>
    /// Require Uppercase.
    /// </summary>
    public virtual bool RequireUppercase { get; set; } = false;

    /// <summary>
    /// Required Length.
    /// </summary>
    public virtual int RequiredLength { get; set; } = 5;

    /// <summary>
    /// Required Unique Characters.
    /// </summary>
    public virtual int RequiredUniqueCharacters { get; set; } = 0;
}