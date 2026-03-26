using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External login provider using Facebook authentication.
/// </summary>
public class ExternalProviderFacebook() : BaseExternalProvider(BuiltInExternalLogInProviderNames.FACEBOOK);