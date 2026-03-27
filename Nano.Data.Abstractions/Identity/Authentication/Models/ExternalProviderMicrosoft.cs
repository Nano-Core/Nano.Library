using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External provider using Microsoft authentication.
/// </summary>
public class ExternalProviderMicrosoft() : BaseExternalProvider<AuthCodeFlow>(BuiltInExternalLogInProviderNames.MICROSOFT);