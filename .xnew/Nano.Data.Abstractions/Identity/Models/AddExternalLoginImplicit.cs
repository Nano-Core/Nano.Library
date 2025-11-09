using System;

namespace Nano.Security.Models;

/// <inheritdoc />
public class AddExternalLoginImplicit<TProvider, TIdentity> : BaseAddExternalLogin<TProvider, TIdentity>
    where TProvider : ExternalLoginProviderImplicit, new()
    where TIdentity : IEquatable<TIdentity>;