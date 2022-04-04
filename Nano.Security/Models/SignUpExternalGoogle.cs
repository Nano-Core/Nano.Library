using System;
using Nano.Models.Interfaces;

namespace Nano.Security.Models
{
    /// <summary>
    /// Sign Up External Google.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TIdentity"></typeparam>
    public class SignUpExternalGoogle<TUser, TIdentity> : SignUpExternalImplicit<LoginExternalProviderGoogle, TUser, TIdentity>
        where TUser : IEntityUser<TIdentity>, new()
        where TIdentity : IEquatable<TIdentity>
    {

    }
}