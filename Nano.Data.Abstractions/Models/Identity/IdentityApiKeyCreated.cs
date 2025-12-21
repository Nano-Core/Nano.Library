using System;
using Nano.Data.Abstractions.Annotations;

namespace Nano.Data.Abstractions.Models.Identity;

/// <summary>
/// Identity Api Key Created.
/// </summary>
public class IdentityApiKeyCreated<TIdentity> : IdentityApiKey<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Identity Api Key.
    /// </summary>
    [Include]
    public virtual IdentityApiKey<TIdentity> IdentityApiKey { get; set; }

    /// <summary>
    /// Unencrypted Hash.
    /// </summary>
    public virtual string UnencryptedHash { get; set; }
}