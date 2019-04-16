using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Change Username.
    /// </summary>
    public class SetUsername
    {
        /// <summary>
        /// User Id.
        /// </summary>
        [Required]
        public virtual string UserId { get; set; }

        /// <summary>
        /// Username.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public virtual string NewUsername { get; set; }
    }
}