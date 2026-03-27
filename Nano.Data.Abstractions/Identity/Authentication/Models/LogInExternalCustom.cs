namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External login request using custom authentication.
/// </summary>
public class LogInExternalCustom<TFlow> : BaseLogInExternal<ExternalProviderCustom<TFlow>, TFlow>
    where TFlow : BaseAuthFlow;