using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Adds a Microsoft external login to an existing user.
/// </summary>
public class AddExternalLoginMicrosoft : BaseAddExternalLogin<ExternalProviderMicrosoft, AuthCodeFlow>;