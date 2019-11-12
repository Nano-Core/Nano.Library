using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Login External Provider.
    /// </summary>
    public class LoginExternalProvider
    {
        /// <summary>
        /// Login Provider.
        /// </summary>
        [Required]
        public virtual string LoginProvider { get; set; }

        /// <summary>
        /// Provider Key.
        /// </summary>
        [Required]
        public virtual string ProviderKey { get; set; }
        
        /// <summary>
        /// Access Token.
        /// </summary>
        [Required]
        public virtual string AccessToken { get; set; }
    }
}