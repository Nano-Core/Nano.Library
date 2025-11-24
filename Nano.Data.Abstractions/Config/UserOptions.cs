using Nano.Data.Abstractions.Identity.Consts;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// User Options (nested class).
/// </summary>
public class UserOptions
{
    /// <summary>
    /// Is Unique Email Address Required.
    /// </summary>
    [Required]
    public virtual bool IsUniqueEmailAddressRequired { get; set; } = true;

    /// <summary>
    /// Is Unique Phone Number Required.
    /// </summary>
    [Required]
    public virtual bool IsUniquePhoneNumberRequired { get; set; } = false;

    /// <summary>
    /// Allowed User Name Characters.
    /// Defaults to abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+
    /// </summary>
    [Required]
    public virtual string AllowedUserNameCharacters { get; set; } = null;

    /// <summary>
    /// Default Roles.
    /// </summary>
    [Required]
    public virtual string[] DefaultRoles { get; set; } =
    [
        BuiltInUserRoles.READER,
        BuiltInUserRoles.WRITER,
        BuiltInUserRoles.SERVICE
    ];
}