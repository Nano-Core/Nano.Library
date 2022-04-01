using System;
using Nano.Models.Interfaces;

namespace Nano.Security.Models
{
    /// <summary>
    /// Sign Up External.
    /// </summary>
    /// <typeparam name="TUser">The user type</typeparam>
    public class SignUpExternalAuthCode<TUser> : SignUpExternalAuthCode<TUser, Guid>
        where TUser : IEntityUser<Guid>
    {

    }

    /// <summary>
    /// Sign Up External.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <typeparam name="TIdentity">The identity type.</typeparam>
    public class SignUpExternalAuthCode<TUser, TIdentity> : BaseSignUpExternal<LoginExternalProviderAuthCode>
        where TUser : IEntityUser<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// User.
        /// </summary>
        public virtual TUser User { get; set; }
    }
}