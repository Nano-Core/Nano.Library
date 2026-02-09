namespace Nano.Common.Consts;

/// <summary>
/// Defines constants for Nano HTTP header names.
/// </summary>
public static class NanoHeaderNames
{
    /// <summary>
    /// The HTTP header name used to pass the API key in requests.
    /// Typically included in the request as <c>x-api-key</c>.
    /// </summary>
    public const string X_API_KEY = "x-api-key";

    /// <summary>
    /// The HTTP header name for a request id genereated for a request.
    /// </summary>
    public const string REQUEST_ID = "RequestId";

    /// <summary>
    /// The HTTP header name for the suppported api versions.
    /// </summary>
    public const string API_SUPPORTED_VERSIONS = "api-supported-versions";
}