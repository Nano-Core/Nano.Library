using System;
using Nano.App.ApiClient.Models.Identity.External.Providers;

namespace Nano.App.ApiClient.Models.Identity;

/// <inheritdoc />
public class AddExternalLoginImplicit<TProvider, TIdentity> : BaseAddExternalLogin<TProvider, TIdentity>
    where TProvider : ExternalLoginProviderImplicit, new()
    where TIdentity : IEquatable<TIdentity>;