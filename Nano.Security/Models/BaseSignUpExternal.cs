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
    /// <typeparam name="TProvider">The provider type.</typeparam>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TIdentity"></typeparam>
    public abstract class BaseSignUpExternal<TProvider, TUser, TIdentity> : BaseSignUpExternal
        where TProvider : BaseLoginExternalProvider, new()
        where TUser : IEntityUser<TIdentity>, new()
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// User.
        /// </summary>
        public virtual TUser User { get; set; } = new();

        /// <summary>
        /// Login External.
        /// </summary>
        public TProvider Provider { get; set; } = new();
    }
}