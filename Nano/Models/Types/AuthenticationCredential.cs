using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Types
{
    // TODO: Add Map extensions method. Add data annotations for validation.
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