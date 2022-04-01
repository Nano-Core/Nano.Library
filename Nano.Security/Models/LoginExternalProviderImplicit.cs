using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Login External Provider Implicit.
    /// </summary>
    public class LoginExternalProviderImplicit : BaseLoginExternalProvider
    {
        /// <summary>
        /// Access Token.
        /// </summary>
        [Required]
        public virtual string AccessToken { get; set; }
    }
}