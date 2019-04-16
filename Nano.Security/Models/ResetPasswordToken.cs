using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Reset Password Token.
    /// </summary>
    public class ResetPasswordToken
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
    }
}