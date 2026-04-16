using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Class for Google external login requests.
/// </summary>
public class LogInExternalGoogleRequest() : LogInExternalImplicitRequest(BuiltInExternalLogInProviderNames.GOOGLE);