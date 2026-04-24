using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Class for auth-code external login requests.
/// </summary>
public class LogInExternalAuthCodeRequest(string providerName) : BaseLogInExternalRequest<AuthCodeFlow>(providerName);