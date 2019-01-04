using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Auth
{
    /// <summary>
    /// Reset Password.
    /// </summary>
    public class ResetPassword
    {
        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [EmailAddress]
        public virtual string Email { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        [Required]
        [MaxLength(128)]
        public virtual string Password { get; set; }

        /// <summary>
        /// Confirm Password.
        /// </summary>
        [Required]
        [MaxLength(128)]
        [Compare("Password")]
        public virtual string ConfirmPassword { get; set; }

        /// <summary>
        /// Code.
        /// </summary>
        [Required]
        public virtual string Code { get; set; }
    }
}