using System;
using System.ComponentModel.DataAnnotations;
using Nano.Models.Interfaces;

namespace Nano.Security.Models
{
    /// <summary>
    /// Sign Up External.
    /// </summary>
    public class SignUpExternal : BaseSignUp
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
    public class SignUpExternal<TUser> : SignUpExternal<TUser, Guid>
        where TUser : IEntityUser<Guid>
    {

    }

    /// <summary>
    /// Sign Up External.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <typeparam name="TIdentity">The identity type.</typeparam>
    public class SignUpExternal<TUser, TIdentity> : SignUpExternal
        where TUser : IEntityUser<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// User.
        /// </summary>
        public virtual TUser User { get; set; }
    }
}