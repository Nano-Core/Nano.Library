using System;
using Nano.Common.Identity.Authentication.Models;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Sign Up External Direct.
/// </summary>
/// <typeparam name="TUser">The user type</typeparam>
public class SignUpExternalDirect<TUser> : BaseSignUpExternal<TUser, Guid>
    where TUser : IEntityUser<Guid>, new();

/// <summary>
/// Sign Up External Direct.
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TIdentity">The identity type.</typeparam>
public class SignUpExternalDirect<TUser, TIdentity> : BaseSignUpExternal<TUser, TIdentity>
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// External Log In Data.
    /// </summary>
    public virtual ExternalLogInData ExternalLogInData { get; set; }
}