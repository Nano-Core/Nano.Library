using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Class for Microsoft transient external login refresh requests.
/// </summary>
public class LogInExternalTransientMicrosoftRefreshRequest() : BaseLogInExternalTransientRefreshRequest(BuiltInExternalLogInProviderNames.MICROSOFT);
