using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Change Email.
    /// </summary>
    public class ChangeEmail
    {
        /// <summary>
        /// User Id.
        /// </summary>
        [Required]
        public virtual string UserId { get; set; }

        /// <summary>
        /// Token.
        /// </summary>
        [Required]
        public virtual string Token { get; set; }

        /// <summary>
        /// New Email.
        /// </summary>
        [Required]
        [EmailAddress]
        public virtual string NewEmail { get; set; }
    }
}