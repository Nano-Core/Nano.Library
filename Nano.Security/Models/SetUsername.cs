using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Set Username.
    /// </summary>
    public class SetUsername
    {
        /// <summary>
        /// User Id.
        /// </summary>
        [Required]
        public virtual string UserId { get; set; }

        /// <summary>
        /// New Username.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public virtual string NewUsername { get; set; }
    }
}