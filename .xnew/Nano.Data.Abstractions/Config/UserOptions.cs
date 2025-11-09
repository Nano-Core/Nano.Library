using Nano.Security.Const;

namespace Nano.Data;

/// <summary>
/// User Options (nested class).
/// </summary>
public class UserOptions
{
    /// <summary>
    /// Is Unique Email Address Required.
    /// </summary>
    public virtual bool IsUniqueEmailAddressRequired { get; set; } = true;

    /// <summary>
    /// Is Unique Phone Number Required.
    /// </summary>
    public virtual bool IsUniquePhoneNumberRequired { get; set; } = false;

    /// <summary>
    /// Allowed User Name Characters.
    /// Defaults to abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+
    /// </summary>
    public virtual string AllowedUserNameCharacters { get; set; } = null;

    /// <summary>
    /// Admin Password.
    /// </summary>
    public virtual string AdminPassword { get; set; }

    /// <summary>
    /// Admin Email Address.
    /// </summary>
    public virtual string AdminEmailAddress { get; set; }

    /// <summary>
    /// Default Roles.
    /// </summary>
    public virtual string[] DefaultRoles { get; set; } =
    [
        BuiltInUserRoles.READER,
        BuiltInUserRoles.WRITER,
        BuiltInUserRoles.SERVICE
    ];
}