using System.Collections.Generic;

namespace Nano.Security.Models
{
    /// <summary>
    /// Login External Transient.
    /// </summary>
    public class LoginExternalTransient : LoginExternalProvider
    {
        /// <summary>
        /// Roles.
        /// </summary>
        public virtual IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}