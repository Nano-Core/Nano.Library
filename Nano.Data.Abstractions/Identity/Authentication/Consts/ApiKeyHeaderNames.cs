namespace Nano.Data.Abstractions.Identity.Authentication.Consts;

/// <summary>
/// Defines constants for HTTP header names used for API key authentication.
/// </summary>
public static class ApiKeyHeaderNames
{
    /// <summary>
    /// The HTTP header name used to pass the API key in requests.
    /// Typically included in the request as <c>x-api-key</c>.
    /// </summary>
    public const string X_API_KEY = "x-api-key";
}