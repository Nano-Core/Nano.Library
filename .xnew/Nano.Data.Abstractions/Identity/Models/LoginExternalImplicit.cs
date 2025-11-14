namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Log In External Implicit.
/// </summary>
public class LogInExternalImplicit<TProvider> : BaseLogInExternal<TProvider>
    where TProvider : ExternalLoginProviderImplicit, new();