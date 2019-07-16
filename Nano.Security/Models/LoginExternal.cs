using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Login External.
    /// </summary>
    public class LoginExternal
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

        /// <summary>
        /// Is Remember Me.
        /// </summary>
        [Required]
        [DefaultValue(false)]
        public virtual bool IsRememerMe { get; set; } = false;
    }
}