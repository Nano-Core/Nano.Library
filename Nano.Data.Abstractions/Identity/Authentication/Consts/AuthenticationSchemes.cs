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
    /// Combined scheme that allows either JWT or API key.
    /// </summary>
    public const string JWT_OR_APIKEY = "JwtOrApiKey";
}