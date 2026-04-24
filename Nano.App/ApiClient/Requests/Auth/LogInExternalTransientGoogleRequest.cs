using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Class for Google transient external login requests.
/// </summary>
public class LogInExternalTransientGoogleRequest() : LogInExternalTransientImplicitRequest(BuiltInExternalLogInProviderNames.GOOGLE);