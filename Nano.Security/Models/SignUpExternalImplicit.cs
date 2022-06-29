using System;
using Nano.Models.Interfaces;

namespace Nano.Security.Models;

/// <summary>
/// Sign Up External Implicit.
/// </summary>
/// <typeparam name="TProvider">The provider type.</typeparam>
/// <typeparam name="TUser">The user type</typeparam>
public class SignUpExternalImplicit<TProvider, TUser> : SignUpExternalImplicit<TProvider, TUser, Guid>
    where TProvider : LogInExternalProviderImplicit, new()
    where TUser : IEntityUser<Guid>, new()
{

}

/// <summary>
/// Sign Up External Implicit.
/// </summary>
/// <typeparam name="TProvider">The provider type.</typeparam>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TIdentity">The identity type.</typeparam>
public class SignUpExternalImplicit<TProvider, TUser, TIdentity> : BaseSignUpExternal<TProvider, TUser, TIdentity>
    where TProvider : LogInExternalProviderImplicit, new()
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>
{

}