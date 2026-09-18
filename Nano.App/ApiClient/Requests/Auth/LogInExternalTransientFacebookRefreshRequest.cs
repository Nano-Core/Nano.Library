using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Class for Facebook transient external login refresh requests.
/// </summary>
public class LogInExternalTransientFacebookRefreshRequest() : BaseLogInExternalTransientRefreshRequest(BuiltInExternalLogInProviderNames.FACEBOOK);
