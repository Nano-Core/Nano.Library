using System;
using Microsoft.AspNetCore.Identity;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Models.Identity;

/// <summary>
/// Identity User Token Expiry.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class IdentityUserTokenExpiry<TIdentity> : IdentityUserToken<TIdentity>, IEntityAuditableNegated
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Expire At.
    /// </summary>
    public virtual DateTimeOffset? ExpireAt { get; set; }
}