using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Confirm Phone Number Token.
    /// </summary>
    public class ConfirmPhoneNumberToken
    {
        /// <summary>
        /// Token.
        /// </summary>
        [Required]
        public virtual string Token { get; set; }

        /// <summary>
        /// Phone Number.
        /// </summary>
        [Required]
        public virtual string PhoneNumber { get; set; }
    }
}