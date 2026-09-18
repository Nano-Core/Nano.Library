using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Class for Google transient external login refresh requests.
/// </summary>
public class LogInExternalTransientGoogleRefreshRequest() : BaseLogInExternalTransientRefreshRequest(BuiltInExternalLogInProviderNames.GOOGLE);
