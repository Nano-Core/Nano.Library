namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External login request using Microsoft authentication and authorization code flow.
/// </summary>
public class LogInExternalMicrosoft : BaseLogInExternal<ExternalProviderMicrosoft, AuthCodeFlow>;