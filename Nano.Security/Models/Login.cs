using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Login.
    /// </summary>
    public class Login
    {
        /// <summary>
        /// App Id.
        /// </summary>
        [MaxLength(256)]
        public virtual string AppId { get; set; }

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

        /// <summary>
        /// Is Remember Me.
        /// </summary>
        [Required]
        [DefaultValue(false)]
        public virtual bool IsRememerMe { get; set; } = false;

        /// <summary>
        /// Is Refreshable.
        /// </summary>
        [Required]
        [DefaultValue(true)]
        public virtual bool IsRefreshable { get; set; } = true;
    }
}