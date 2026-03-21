namespace Nano.Data.Abstractions.Identity.Authentication.Consts;

/// <summary>
/// Defines constants for authentication scheme names used in the application.
/// These can be referenced when configuring authentication policies or validating the authentication type.
/// </summary>
public static class AuthenticationSchemes
{
    /// <summary>
    /// JWT bearer authentication scheme.
    /// </summary>
    public const string JWT = "Jwt";

    /// <summary>
    /// API key authentication scheme.
    /// </summary>
    public const string API_KEY = "ApiKey";

    /// <summary>
    /// Combined scheme that allows either JWT, API key or custom authentication.
    /// </summary>
    public const string DYNAMIC_SCHEME = "DynamicScheme";
}