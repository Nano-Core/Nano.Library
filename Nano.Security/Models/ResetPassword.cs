using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Reset Password.
    /// </summary>
    public class ResetPassword
    {
        /// <summary>
        /// User Id.
        /// </summary>
        [Required]
        public virtual string UserId { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public virtual string Password { get; set; }

        /// <summary>
        /// Token.
        /// </summary>
        [Required]
        public virtual string Token { get; set; }
    }
}