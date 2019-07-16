using System.ComponentModel.DataAnnotations;
using Nano.Models;

namespace Nano.Security.Models
{
    /// <summary>
    /// Sign Up External.
    /// </summary>
    public class SignUpExternal
    {
        /// <summary>
        /// Email Address.
        /// </summary>
        [Required]
        [EmailAddress]
        public virtual string EmailAddress { get; set; }

        /// <summary>
        /// External Login.
        /// </summary>
        [Required]
        public virtual LoginExternal ExternalLogin { get; set; } = new LoginExternal();
    }

    /// <summary>
    /// Sign Up External.
    /// </summary>
    /// <typeparam name="TUser">The user type</typeparam>
    public class SignUpExternal<TUser> : SignUpExternal
        where TUser : DefaultEntityUser
    {
        /// <summary>
        /// User.
        /// </summary>
        public virtual TUser User { get; set; }
    }
}