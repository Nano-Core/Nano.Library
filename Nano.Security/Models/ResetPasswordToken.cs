using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Reset Password Token.
    /// </summary>
    public class ResetPasswordToken
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
    }
}