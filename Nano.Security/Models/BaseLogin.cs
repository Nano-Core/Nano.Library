using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Base LogIn (abstract).
    /// </summary>
    public abstract class BaseLogIn
    {
        /// <summary>
        /// App Id.
        /// </summary>
        [MaxLength(256)]
        public virtual string AppId { get; set; }

        /// <summary>
        /// Is Remember Me.
        /// Not relevant for transient logins.
        /// </summary>
        [Required]
        [DefaultValue(false)]
        public virtual bool IsRememberMe { get; set; } = false;

        /// <summary>
        /// Is Refreshable.
        /// Not relevant for transient logins.
        /// </summary>
        [Required]
        [DefaultValue(true)]
        public virtual bool IsRefreshable { get; set; } = false;

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