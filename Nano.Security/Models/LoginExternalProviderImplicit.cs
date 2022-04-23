using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// LogIn External Provider Implicit (abstract).
    /// </summary>
    public abstract class LogInExternalProviderImplicit : BaseLogInExternalProvider
    {
        /// <summary>
        /// Access Token.
        /// </summary>
        [Required]
        public virtual string AccessToken { get; set; }
    }
}