using System;

namespace Nano.Security.Models;

/// <summary>
/// Add External Login Google.
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public class AddExternalLoginGoogle<TIdentity> : AddExternalLoginImplicit<ExternalLoginProviderGoogle, TIdentity>
    where TIdentity : IEquatable<TIdentity>;