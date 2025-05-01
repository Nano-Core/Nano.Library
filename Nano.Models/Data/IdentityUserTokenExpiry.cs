using System;
using Microsoft.AspNetCore.Identity;

namespace Nano.Models.Data;

/// <summary>
/// Identity User Token Expiry.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class IdentityUserTokenExpiry<TIdentity> : IdentityUserToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Expire At.
    /// </summary>
    public virtual DateTimeOffset? ExpireAt { get; set; }
}