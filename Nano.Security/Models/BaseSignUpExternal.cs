using System;
using Nano.Models.Interfaces;

namespace Nano.Security.Models
{
    /// <summary>
    /// Base Sign-Up External (abstract).
    /// </summary>
    public abstract class BaseSignUpExternal : BaseSignUp
    {

    }

    /// <summary>
    /// Base Sign-Up External (abstract).
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <typeparam name="TIdentity">The identity type.</typeparam>
    public abstract class BaseSignUpExternal<TUser, TIdentity> : BaseSignUpExternal
        where TUser : IEntityUser<TIdentity>, new()
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// User.
        /// </summary>
        public virtual TUser User { get; set; } = new();
    }

    /// <summary>
    /// Base Sign-Up External (abstract).
    /// </summary>
    /// <typeparam name="TProvider">The provider type.</typeparam>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <typeparam name="TIdentity">The identity type.</typeparam>
    public abstract class BaseSignUpExternal<TProvider, TUser, TIdentity> : BaseSignUpExternal<TUser, TIdentity>
        where TProvider : BaseLogInExternalProvider, new()
        where TUser : IEntityUser<TIdentity>, new()
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// Login External.
        /// </summary>
        public TProvider Provider { get; set; } = new();
    }
}