using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Password Options (nested class).
/// </summary>
public class PasswordOptions
{
    /// <summary>
    /// Require Digit.
    /// </summary>
    [Required]
    public virtual bool RequireDigit { get; set; } = false;

    /// <summary>
    /// Require Non Alphanumeric.
    /// </summary>
    [Required]
    public virtual bool RequireNonAlphanumeric { get; set; } = false;

    /// <summary>
    /// Require Lowercase.
    /// </summary>
    [Required]
    public virtual bool RequireLowercase { get; set; } = false;

    /// <summary>
    /// Require Uppercase.
    /// </summary>
    [Required]
    public virtual bool RequireUppercase { get; set; } = false;

    /// <summary>
    /// Required Length.
    /// </summary>
    [Required]
    public virtual int RequiredLength { get; set; } = 5;

    /// <summary>
    /// Required Unique Characters.
    /// </summary>
    [Required]
    public virtual int RequiredUniqueCharacters { get; set; } = 0;
}