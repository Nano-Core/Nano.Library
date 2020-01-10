using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Refresh Token.
    /// </summary>
    public class RefreshToken
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
        public virtual DateTimeOffset? ExpireAt { get; set; }

        /// <summary>
        /// Is Expired.
        /// </summary>
        public virtual bool IsExpired => this.ExpireAt <= DateTimeOffset.UtcNow;
    }
}