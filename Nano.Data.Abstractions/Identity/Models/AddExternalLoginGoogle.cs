using System;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Add External Login Google.
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public class AddExternalLoginGoogle<TIdentity> : AddExternalLoginImplicit<ExternalLoginProviderGoogle, TIdentity>
    where TIdentity : IEquatable<TIdentity>;