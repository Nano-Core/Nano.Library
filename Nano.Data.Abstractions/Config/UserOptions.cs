using Nano.Data.Abstractions.Identity.Consts;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Options for user-specific settings.
/// </summary>
public class UserOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether each user must have a unique email address.
    /// Defaults to <c>true</c>.
    /// </summary>
    [Required]
    public virtual bool IsUniqueEmailAddressRequired { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether each user must have a unique phone number.
    /// Defaults to <c>false</c>.
    /// </summary>
    [Required]
    public virtual bool IsUniquePhoneNumberRequired { get; set; } = false;

    /// <summary>
    /// Gets or sets the allowed characters for usernames.
    /// Defaults to "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+".
    /// </summary>
    public virtual string AllowedUserNameCharacters { get; set; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    /// <summary>
    /// Gets or sets the default roles assigned to a new user.
    /// </summary>
    [Required]
    public virtual string[] DefaultRoles { get; set; } =
    [
        BuiltInUserRoles.ADMINISTRATOR
    ];
}