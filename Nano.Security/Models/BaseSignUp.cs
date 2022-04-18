using System.Collections.Generic;

namespace Nano.Security.Models
{
    /// <summary>
    /// Base Sign-Up (abstract).
    /// </summary>
    public abstract class BaseSignUp
    {
        /// <summary>
        /// Roles.
        /// Additional Roles to add to the user, besides the default roles in the configuration.
        /// </summary>
        public virtual IEnumerable<string> Roles { get; set; } = new List<string>();

        /// <summary>
        /// Claims.
        /// Additonal claims to add to the user.
        /// </summary>
        public virtual IDictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
    }
}