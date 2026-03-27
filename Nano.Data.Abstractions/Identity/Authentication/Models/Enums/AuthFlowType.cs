namespace Nano.Data.Abstractions.Identity.Authentication.Models.Enums;

/// <summary>
/// Specifies the type of authentication flow used when performing external authentication.
/// </summary>
public enum AuthFlowType
{
    /// <summary>
    /// Represents the implicit flow, where an access token is returned directly from the external provider.
    /// </summary>
    Implicit = 0,

    /// <summary>
    /// Represents the authorization code flow, where an authorization code is exchanged for tokens.
    /// </summary>
    AuthCode = 1
}