using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Login Refresh.
    /// </summary>
    public class LoginRefresh
    {
        /// <summary>
        /// Token.
        /// </summary>
        [Required]
        public virtual string Token { get; set; }

        /// <summary>
        /// Refresh Token.
        /// </summary>
        [Required]
        public virtual string RefreshToken { get; set; }

        /// <summary>
        /// Transient Claims.
        /// </summary>
        public virtual IDictionary<string, string> TransientClaims { get; set; } = new Dictionary<string, string>();
    }
}