using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Login External Transient.
    /// </summary>
    public class LoginExternalTransient : LoginExternalProvider
    {
        /// <summary>
        /// App Id.
        /// </summary>
        [MaxLength(256)]
        public virtual string AppId { get; set; }

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