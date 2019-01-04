using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Auth
{
    /// <summary>
    /// Send Reset Password
    /// </summary>
    public class GetResetPassword
    {
        /// <summary>
        /// Email.
        /// </summary>
        [Required]
        [EmailAddress]
        public virtual string Email { get; set; }
    }
}