using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Class for implicit external login requests.
/// </summary>
public class LogInExternalImplicitRequest(string providerName) : BaseLogInExternalRequest<ImplicitFlow>(providerName);