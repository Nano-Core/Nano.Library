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
        [MaxLength(256)]
        public virtual string Username { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public virtual string Password { get; set; }
    }
}