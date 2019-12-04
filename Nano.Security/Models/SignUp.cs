using System.ComponentModel.DataAnnotations;
using Nano.Models;

namespace Nano.Security.Models
{
    /// <summary>
    /// Sign Up.
    /// </summary>
    public class SignUp : BaseSignUp
    {
        /// <summary>
        /// Email.
        /// </summary>
        [Required]
        [EmailAddress]
        public virtual string EmailAddress { get; set; }

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
        /// Confirm Password. 
        /// </summary>
        [Required]
        [MaxLength(256)]
        [Compare("Password")]
        public virtual string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// Sign Up.
    /// </summary>
    /// <typeparam name="TUser">The user type</typeparam>
    public class SignUp<TUser> : SignUp
        where TUser : DefaultEntityUser
    {
        /// <summary>
        /// User.
        /// </summary>
        public virtual TUser User { get; set; }
    }
}
