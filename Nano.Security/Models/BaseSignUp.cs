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
        /// </summary>
        public virtual IEnumerable<string> Roles { get; set; } = new List<string>();

        /// <summary>
        /// Claims.
        /// </summary>
        public virtual IDictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
    }
}