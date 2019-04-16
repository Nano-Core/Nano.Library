using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Login External.
    /// </summary>
    public class LoginExternal
    {
        /// <summary>
        /// Provider Key.
        /// The unique key for the user provided by the login provider.
        /// </summary>
        [Required]
        public virtual string ProviderKey { get; set; }

        /// <summary>
        /// Login Provider.
        /// The name of the login provider.
        /// </summary>
        [Required]
        public virtual string LoginProvider { get; set; }
    }
}