using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Class for Microsoft transient external login requests.
/// </summary>
public class LogInExternalTransientMicrosoftRequest() : LogInExternalTransientAuthCodeRequest(BuiltInExternalLogInProviderNames.MICROSOFT);