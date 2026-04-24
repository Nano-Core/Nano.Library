using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Class for implicit transient external login requests.
/// </summary>
public class LogInExternalTransientImplicitRequest(string providerName) : BaseLogInExternalTransientRequest<ImplicitFlow>(providerName);