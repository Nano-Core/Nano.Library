using System;
using Nano.Models.Attributes;

namespace Nano.Security.Data.Models;

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