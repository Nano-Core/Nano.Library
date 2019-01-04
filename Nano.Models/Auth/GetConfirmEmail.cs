using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Auth
{
    /// <summary>
    /// Get Confirm Email.
    /// </summary>
    public class GetConfirmEmail
    {
        /// <summary>
        /// User Id.
        /// </summary>
        [Required]
        public string UserId { get; set; }
    }
}