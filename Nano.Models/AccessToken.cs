using System;

namespace Nano.Models
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
        /// ExpireAt.
        /// </summary>
        public virtual DateTime ExpireAt { get; set; }

        /// <summary>
        /// Is Valid.
        /// </summary>
        public virtual bool IsValid => this.Token != null && this.ExpireAt > DateTimeOffset.UtcNow;

    }
}