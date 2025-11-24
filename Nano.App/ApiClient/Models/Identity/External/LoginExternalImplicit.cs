using Nano.App.ApiClient.Models.Identity.External.Providers;

namespace Nano.App.ApiClient.Models.Identity.External;

/// <summary>
/// Log In External Implicit.
/// </summary>
public class LogInExternalImplicit<TProvider> : BaseLogInExternal<TProvider>
    where TProvider : ExternalLoginProviderImplicit, new();