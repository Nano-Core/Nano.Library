using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Set Password.
    /// </summary>
    public class SetPassword
    {
        /// <summary>
        /// User Id.
        /// </summary>
        [Required]
        public virtual string UserId { get; set; }

        /// <summary>
        /// New Password.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public virtual string NewPassword { get; set; }
    }
}