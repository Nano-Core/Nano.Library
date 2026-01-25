namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External login request using an implicit authentication flow.
/// </summary>
public class LogInExternalImplicit<TProvider> : BaseLogInExternal<TProvider>
    where TProvider : ExternalLoginProviderImplicit, new();