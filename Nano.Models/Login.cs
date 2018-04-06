using System.ComponentModel.DataAnnotations;

namespace Nano.Models
{
    /// <summary>
    /// Login.
    /// </summary>
    public class Login
    {
        /// <summary>
        /// Username (Email).
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