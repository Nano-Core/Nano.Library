using System;

namespace Nano.Security.Models
{
    /// <summary>
    /// Access Token.
    /// </summary>
    public class AccessToken
    {
        /// <summary>
        /// Token.
        /// </summary>
        public virtual string Token { get; set; }

        /// <summary>
        /// Expire At.
        /// </summary>
        public virtual DateTime ExpireAt { get; set; }

        /// <summary>
        /// Is Expired.
        /// </summary>
        public virtual bool IsExpired => this.Token != null && this.ExpireAt > DateTimeOffset.UtcNow;
    }
}