namespace Nano.Data.Abstractions.Identity.Consts;

/// <summary>
/// Defines built-in user roles for identity management and authorization.
/// </summary>
public static class BuiltInUserRoles
{
    /// <summary>
    /// Role for read-only access.
    /// </summary>
    public const string READER = "reader";

    /// <summary>
    /// Role for creating content.
    /// </summary>
    public const string CREATOR = "creator";

    /// <summary>
    /// Role for editing content.
    /// </summary>
    public const string EDITOR = "editor";

    /// <summary>
    /// Role for deleting content.
    /// </summary>
    public const string DELETER = "deleter";

    /// <summary>
    /// Role for writing content.
    /// </summary>
    public const string WRITER = "writer";

    /// <summary>
    /// Role for identity management.
    /// </summary>
    public const string IDENTITY = "identity";

    /// <summary>
    /// Role for full administrative access.
    /// </summary>
    public const string ADMINISTRATOR = "administrator";
}