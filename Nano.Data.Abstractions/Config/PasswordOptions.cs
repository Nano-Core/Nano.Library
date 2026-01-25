using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Options for password complexity requirements.
/// </summary>
public class PasswordOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the password must contain at least one digit.
    /// Defaults to <c>false</c>.
    /// </summary>
    [Required]
    public virtual bool RequireDigit { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the password must contain at least one non-alphanumeric character.
    /// Defaults to <c>false</c>.
    /// </summary>
    [Required]
    public virtual bool RequireNonAlphanumeric { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the password must contain at least one lowercase letter.
    /// Defaults to <c>false</c>.
    /// </summary>
    [Required]
    public virtual bool RequireLowercase { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the password must contain at least one uppercase letter.
    /// Defaults to <c>false</c>.
    /// </summary>
    [Required]
    public virtual bool RequireUppercase { get; set; } = false;

    /// <summary>
    /// Gets or sets the minimum required length of the password.
    /// Defaults to <c>5</c>.
    /// </summary>
    [Required]
    public virtual int RequiredLength { get; set; } = 5;

    /// <summary>
    /// Gets or sets the number of unique characters required in the password.
    /// Defaults to <c>0</c>.
    /// </summary>
    [Required]
    public virtual int RequiredUniqueCharacters { get; set; } = 0;
}