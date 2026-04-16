using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Class for Facebook transient external login requests.
/// </summary>
public class LogInExternalTransientFacebookRequest() : LogInExternalTransientImplicitRequest(BuiltInExternalLogInProviderNames.FACEBOOK);