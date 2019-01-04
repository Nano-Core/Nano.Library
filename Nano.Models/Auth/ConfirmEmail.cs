using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Auth
{
    /// <summary>
    /// Confirm Email.
    /// </summary>
    public class ConfirmEmail
    {
        /// <summary>
        /// Code.
        /// </summary>
        [Required]
        public virtual string Code { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        [Required]
        public virtual string UserId { get; set; }

    }
}