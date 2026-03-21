using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Adds an external login using the authorization code flow.
/// </summary>
/// <typeparam name="TProvider">The external authentication provider type.</typeparam>
public class AddExternalLoginAuthCode<TProvider> : BaseAddExternalLogin<TProvider>
    where TProvider : ExternalLoginProviderAuthCode, new();