using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Change Email Token
    /// </summary>
    public class ChangeEmailToken
    {
        /// <summary>
        /// Token.
        /// </summary>
        [Required]
        public virtual string Token { get; set; }

        /// <summary>
        /// Email Address.
        /// </summary>
        [Required]
        [EmailAddress]
        public virtual string EmailAddress { get; set; }

        /// <summary>
        /// New Email Address.
        /// </summary>
        [Required]
        [EmailAddress]
        public virtual string NewEmailAddress { get; set; }
    }
}