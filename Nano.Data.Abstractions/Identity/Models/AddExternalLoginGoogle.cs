using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Adds a Google external login to an existing user.
/// </summary>
public class AddExternalLoginGoogle : BaseAddExternalLogin<ExternalProviderGoogle, ImplicitFlow>;