using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Types
{
    /// <summary>
    /// Authentication Credential.
    /// </summary>
    public class AuthenticationCredential
    {
        /// <summary>
        /// Username.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public virtual string Username { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public virtual string Password { get; set; }

        /// <summary>
        /// Token.
        /// </summary>
        [MaxLength(255)]
        public virtual string Token { get; set; }
    }
}