using System;
using Microsoft.AspNetCore.Identity;
using Nano.Models.Interfaces;

namespace Nano.Security.Data.Models;

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