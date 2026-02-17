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
    public const string X_API_KEY = "X-Api-Key";

    /// <summary>
    /// The HTTP header name for a request id (<c>X-Request-Id</c>) genereated for a request.
    /// </summary>
    public const string REQUEST_ID = "X-Request-Id";

    /// <summary>
    /// The HTTP header name for the api version.
    /// </summary>
    public const string API_VERSION = "Api-Version";

    /// <summary>
    /// The HTTP header name for the suppported api versions.
    /// </summary>
    public const string API_SUPPORTED_VERSIONS = "Api-Supported-Versions";

    /// <summary>
    /// Identifies the original client IP address as set by a reverse proxy.
    /// Commonly used for logging, auditing, and request correlation.
    /// </summary>
    public const string X_FORWARDED_FOR = "X-Forwarded-For";

    /// <summary>
    /// Identifies the original request scheme (e.g. http or https) as set by a reverse proxy.
    /// Used to reconstruct the externally visible request URL.
    /// </summary>
    public const string X_FORWARDED_PROTO = "X-Forwarded-Proto";

    /// <summary>
    /// Identifies the original host requested by the client as set by a reverse proxy.
    /// Typically used to preserve the public-facing host name.
    /// </summary>
    public const string X_FORWARDED_HOST = "X-Forwarded-Host";

    /// <summary>
    /// Identifies the original port requested by the client as set by a reverse proxy.
    /// Useful when the external port differs from the internal service port.
    /// </summary>
    public const string X_FORWARDED_PORT = "X-Forwarded-Port";

    /// <summary>
    /// Identifies the original request path base (URL prefix) as set by a reverse proxy.
    /// Commonly used when applications are hosted behind a path-based proxy.
    /// </summary>
    public const string X_FORWARDED_PREFIX = "X-Forwarded-Prefix";
}