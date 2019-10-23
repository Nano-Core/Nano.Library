using System;
using Microsoft.AspNetCore.Identity;
using Nano.Models.Interfaces;

namespace Nano.Security.Models
{
    /// <summary>
    /// Identity User Token Expiry.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class IdentityUserTokenExpiry<TKey> : IdentityUserToken<TKey>, IEntityAuditableNegated
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Expire At.
        /// </summary>
        public virtual DateTimeOffset ExpireAt { get; set; }
    }
}