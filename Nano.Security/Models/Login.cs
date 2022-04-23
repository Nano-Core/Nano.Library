using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// LogIn.
    /// </summary>
    public class LogIn : BaseLogIn
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