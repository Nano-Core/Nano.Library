using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class AddExternalLoginImplicit<TProvider> : BaseAddExternalLogin<TProvider>
    where TProvider : ExternalLoginProviderImplicit, new();