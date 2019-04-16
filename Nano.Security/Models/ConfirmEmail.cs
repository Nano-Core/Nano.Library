using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Confirm Email.
    /// </summary>
    public class ConfirmEmail
    {
        /// <summary>
        /// Token.
        /// </summary>
        [Required]
        public virtual string Token { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        [Required]
        [EmailAddress]
        public virtual string EmailAddress { get; set; }
    }
}