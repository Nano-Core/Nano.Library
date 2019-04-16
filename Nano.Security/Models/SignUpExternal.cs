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
        /// Email.
        /// </summary>
        [Required]
        [EmailAddress]
        public virtual string Email { get; set; }

        /// <summary>
        /// Provider Key.
        /// The unique key for the user provided by the login provider.
        /// </summary>
        [Required]
        public virtual string ProviderKey { get; set; }

        /// <summary>
        /// Login Provider.
        /// The name of the login provider.
        /// </summary>
        [Required]
        public virtual string LoginProvider { get; set; }
    }

    /// <summary>
    /// Sign Up External.
    /// </summary>
    /// <typeparam name="TUser">The user type</typeparam>
    public class SignUpExternal<TUser> : SignUpExternal
        where TUser : DefaultEntityUser
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual TUser User { get; set; }
    }

}