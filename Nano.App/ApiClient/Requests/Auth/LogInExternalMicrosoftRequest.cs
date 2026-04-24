using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Class for Microsoft external login requests.
/// </summary>
public class LogInExternalMicrosoftRequest() : LogInExternalAuthCodeRequest(BuiltInExternalLogInProviderNames.MICROSOFT);