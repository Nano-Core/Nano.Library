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
        /// Access Token.
        /// </summary>
        [Required]
        public virtual string AccessToken { get; set; }
    }
}