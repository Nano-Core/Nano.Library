using System;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class AddExternalLoginImplicit<TProvider, TIdentity> : BaseAddExternalLogin<TProvider, TIdentity>
    where TProvider : ExternalLoginProviderImplicit, new()
    where TIdentity : IEquatable<TIdentity>;