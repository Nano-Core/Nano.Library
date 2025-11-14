namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Log In External Auth Code.
/// </summary>
public class LogInExternalAuthCode<TProvider> : BaseLogInExternal<TProvider>
    where TProvider : ExternalLoginProviderAuthCode, new();