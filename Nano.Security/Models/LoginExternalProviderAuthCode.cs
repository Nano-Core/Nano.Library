using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// LogIn External Provider Auth Code (abstract).
    /// </summary>
    public abstract class LogInExternalProviderAuthCode : BaseLogInExternalProvider
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