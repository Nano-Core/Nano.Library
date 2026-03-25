using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Adds a Facebook external login to an existing user.
/// </summary>
public class AddExternalLoginFacebook : BaseAddExternalLogin<ExternalProviderFacebook, ImplicitFlow>;