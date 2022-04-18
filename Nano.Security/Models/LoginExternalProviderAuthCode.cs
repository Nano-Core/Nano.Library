using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Login External Provider Auth Code (abstract).
    /// </summary>
    public abstract class LoginExternalProviderAuthCode : BaseLoginExternalProvider
    {
        /// <summary>
        /// Code.
        /// </summary>
        [Required]
        public virtual string Code { get; set; }

        /// <summary>
        /// Code Verifier.
        /// </summary>
        [Required]
        public virtual string CodeVerifier { get; set; }

        /// <summary>
        /// Redirect Uri.
        /// </summary>
        [Required]
        public virtual string RedirectUri { get; set; }
    }
}