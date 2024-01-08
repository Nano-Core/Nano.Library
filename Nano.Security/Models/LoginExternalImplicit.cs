namespace Nano.Security.Models;

/// <summary>
/// LogIn External Implicit (abstract).
/// </summary>
public abstract class LogInExternalImplicit<TProvider> : BaseLogInExternal<TProvider>
    where TProvider : LogInExternalProviderImplicit, new();