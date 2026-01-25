using System;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Adds an external login using the authorization code flow.
/// </summary>
/// <typeparam name="TProvider">The external authentication provider type.</typeparam>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class AddExternalLoginAuthCode<TProvider, TIdentity> : BaseAddExternalLogin<TProvider, TIdentity>
    where TProvider : ExternalLoginProviderAuthCode, new()
    where TIdentity : IEquatable<TIdentity>;