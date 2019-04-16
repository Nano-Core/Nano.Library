using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Confirm Email Token.
    /// </summary>
    public class ConfirmEmailToken
    {
        /// <summary>
        /// User Id.
        /// </summary>
        [Required]
        public virtual string UserId { get; set; }

        /// <summary>
        /// Token.
        /// </summary>
        [Required]
        public virtual string Token { get; set; }
    }
}