using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Change Password.
    /// </summary>
    public class ChangePassword
    {
        /// <summary>
        /// User Id.
        /// </summary>
        [Required]
        public virtual string UserId { get; set; }

        /// <summary>
        /// Old Password.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public virtual string OldPassword { get; set; }

        /// <summary>
        /// New Password.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public virtual string NewPassword { get; set; }

        /// <summary>
        /// Confirm New Passowrd.
        /// </summary>
        [Required]
        [MaxLength(256)]
        [Compare("NewPassword")]
        public virtual string ConfirmNewPassword { get; set; }
    }
}