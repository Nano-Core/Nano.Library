namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External login request using the authorization code flow.
/// </summary>
public class LogInExternalAuthCode<TProvider> : BaseLogInExternal<TProvider>
    where TProvider : ExternalLoginProviderAuthCode, new();