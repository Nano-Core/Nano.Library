using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External provider using Google authentication.
/// </summary>
public class ExternalProviderGoogle() : BaseExternalProvider<ImplicitFlow>(BuiltInExternalLogInProviderNames.FACEBOOK);