namespace Nano.Data.Abstractions.Identity.Consts;

/// <summary>
/// Provides additional claim type constants for JWT and identity management.
/// </summary>
public static class ClaimTypesExtended
{
    /// <summary>
    /// Claim type for the application identifier.
    /// </summary>
    public static string AppId => "app_id";

    /// <summary>
    /// Claim type for the external provider name.
    /// </summary>
    public static string ExternalProviderName => "external_provider_name";

    /// <summary>
    /// Claim type for the external provider token.
    /// </summary>
    public static string ExternalProviderToken => "external_provider_token";

    /// <summary>
    /// Claim type for the external provider refresh token.
    /// </summary>
    public static string ExternalProviderRefreshToken => "external_provider_refresh_token";

    /// <summary>
    /// Claim type for the API key identifier.
    /// </summary>
    public static string ApiKeyId => "ApiKeyId";

    /// <summary>
    /// Claim type for the API key name.
    /// </summary>
    public static string ApiKeyName => "ApiKeyName";
}