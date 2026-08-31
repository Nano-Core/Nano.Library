namespace Nano.Data.Abstractions.Config.Enums;

/// <summary>
/// The database authentication types.
/// </summary>
public enum AuthenticationType
{
    /// <summary>
    /// Credentials. username / password.
    /// </summary>
    Credentials = 0,

    /// <summary>
    /// Azure authentication using Entra.
    /// </summary>
    Azure = 1
}