using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Change Email.
    /// </summary>
    public class ChangeEmail
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
        public virtual string UserId { get; set; }

        /// <summary>
        /// New Email.
        /// </summary>
        [Required]
        [EmailAddress]
        public virtual string NewEmail { get; set; }
    }
}