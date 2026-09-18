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
    /// Claim type for the manifest recording which roles/claims on this token were asserted as
    /// non-persisted "transient" ones at login, so a refresh can recover and carry forward exactly
    /// that set instead of trusting the refresh caller to resupply it. The value is an opaque,
    /// internally-encoded blob - see <c>TransientClaimsManifest</c> - not meant to be read directly.
    /// </summary>
    public static string TransientClaimsManifest => "transient_claims_manifest";

    /// <summary>
    /// Claim type for the API key identifier.
    /// </summary>
    public static string ApiKeyId => "ApiKeyId";

    /// <summary>
    /// Claim type for the API key name.
    /// </summary>
    public static string ApiKeyName => "ApiKeyName";
}