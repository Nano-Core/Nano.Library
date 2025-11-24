using Nano.App.ApiClient.Models.Identity.External.Providers;

namespace Nano.App.ApiClient.Models.Identity.External;

/// <summary>
/// Log In External Auth Code.
/// </summary>
public class LogInExternalAuthCode<TProvider> : BaseLogInExternal<TProvider>
    where TProvider : ExternalLoginProviderAuthCode, new();