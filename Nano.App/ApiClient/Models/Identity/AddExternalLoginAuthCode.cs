using System;
using Nano.App.ApiClient.Models.Identity.External.Providers;

namespace Nano.App.ApiClient.Models.Identity;

/// <inheritdoc />
public class AddExternalLoginAuthCode<TProvider, TIdentity> : BaseAddExternalLogin<TProvider, TIdentity>
    where TProvider : ExternalLoginProviderAuthCode, new()
    where TIdentity : IEquatable<TIdentity>;