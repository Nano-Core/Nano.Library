using System;
using Nano.Models.Interfaces;

namespace Nano.Security.Models
{
    /// <summary>
    /// Sign Up External Facebook.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TIdentity"></typeparam>
    public class SignUpExternalFacebook<TUser, TIdentity> : SignUpExternalImplicit<LogInExternalProviderFacebook, TUser, TIdentity>
        where TUser : IEntityUser<TIdentity>, new()
        where TIdentity : IEquatable<TIdentity>
    {

    }
}