namespace Nano.Data.Abstractions.Config.Enums;

/// <summary>
/// The startup action for the database.
/// </summary>
public enum DatabaseStartupAction
{
    /// <summary>
    /// No action at startup.
    /// </summary>
    None = 0,

    /// <summary>
    /// The database will be created automatically from scratch if it doesn't exists.
    /// </summary>
    Create = 1,

    /// <summary>
    /// Database migrations will be applied automatically at startup.
    /// </summary>
    Migrate = 1
}