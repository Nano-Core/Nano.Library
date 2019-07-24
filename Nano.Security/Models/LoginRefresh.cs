using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Login Refresh.
    /// </summary>
    public class LoginRefresh
    {
        /// <summary>
        /// Refresh Token.
        /// </summary>
        [Required]
        public virtual string RefreshToken { get; set; }
    }
}