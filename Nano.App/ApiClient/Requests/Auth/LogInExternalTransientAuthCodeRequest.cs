using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Class for auth-code transient external login requests.
/// </summary>
public class LogInExternalTransientAuthCodeRequest(string providerName) : BaseLogInExternalTransientRequest<AuthCodeFlow>(providerName);