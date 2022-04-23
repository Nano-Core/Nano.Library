using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Login Refresh.
    /// </summary>
    public class LogInRefresh
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
        /// Transient Roles.
        /// Non persisted roles, that is added to the jwt-token when logging in.
        /// </summary>
        public virtual IEnumerable<string> TransientRoles { get; set; } = new List<string>();

        /// <summary>
        /// Transient Claims.
        /// Non persisted claims, that is added to the jwt-token when logging in.
        /// </summary>
        public virtual IDictionary<string, string> TransientClaims { get; set; } = new Dictionary<string, string>();
    }
}