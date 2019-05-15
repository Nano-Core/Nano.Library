using System;
using System.ComponentModel.DataAnnotations;

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
        [Required]
        public virtual string Token { get; set; }

        /// <summary>
        /// Expire At.
        /// </summary>
        [Required]
        public virtual DateTime ExpireAt { get; set; }

        /// <summary>
        /// Is Expired.
        /// </summary>
        public virtual bool IsExpired => this.ExpireAt <= DateTimeOffset.UtcNow;
    }
}