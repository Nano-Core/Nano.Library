using System;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Adds a Google external login to an existing user.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class AddExternalLoginGoogle<TIdentity> : AddExternalLoginImplicit<ExternalLoginProviderGoogle, TIdentity>
    where TIdentity : IEquatable<TIdentity>;