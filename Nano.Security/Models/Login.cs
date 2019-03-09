using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Auth
{
    /// <summary>
    /// Login.
    /// </summary>
    public class Login
    {
        /// <summary>
        /// Username.
        /// The username of the user to sign-in.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public virtual string Username { get; set; }

        /// <summary>
        /// Password.
        /// The password of the user to sign-in.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public virtual string Password { get; set; }

        /// <summary>
        /// Is Remember Me.
        /// Flag indicating whether the sign-in cookie should persist after the browser is closed.
        /// </summary>
        [Required]
        [DefaultValue(false)]
        public virtual bool IsRememerMe { get; set; } = false;
    }
}