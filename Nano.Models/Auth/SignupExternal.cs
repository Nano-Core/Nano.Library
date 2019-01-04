using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Auth
{
    /// <summary>
    /// Sign Up External.
    /// </summary>
    public class SignupExternal
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
    }
}