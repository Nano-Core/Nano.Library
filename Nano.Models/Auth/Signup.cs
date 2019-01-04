using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Auth
{
    /// <summary>
    /// Sign Up.
    /// </summary>
    public class Signup
    {
        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [EmailAddress]
        public virtual string Email { get; set; }

        /// <summary>
        /// Username.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public virtual string Username { get; set; }

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
        public virtual string ConfirmPassword { get; set; }
    }
}