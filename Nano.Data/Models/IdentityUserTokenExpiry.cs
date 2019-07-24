using System;
using Microsoft.AspNetCore.Identity;

namespace Nano.Data.Models
{
    /// <inheritdoc />
    public class IdentityUserTokenExpiry<TKey> : IdentityUserToken<TKey> 
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Expire At.
        /// </summary>
        public virtual DateTimeOffset ExpireAt { get; set; }
    }
}