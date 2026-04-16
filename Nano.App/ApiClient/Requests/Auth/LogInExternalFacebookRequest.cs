using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Class for Facebook external login requests.
/// </summary>
public class LogInExternalFacebookRequest() : LogInExternalImplicitRequest(BuiltInExternalLogInProviderNames.FACEBOOK);