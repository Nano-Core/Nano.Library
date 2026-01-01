using System;
using Nano.Common.Identity.Authentication.Models;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Sign Up External.
/// </summary>
/// <typeparam name="TProvider">The provider type.</typeparam>
/// <typeparam name="TUser">The user type</typeparam>
public class SignUpExternalAuthCode<TProvider, TUser> : SignUpExternalAuthCode<TProvider, TUser, Guid>
    where TProvider : ExternalLoginProviderAuthCode, new()
    where TUser : IEntityUser<Guid>, new();

/// <summary>
/// Sign Up External Auth Code.
/// </summary>
/// <typeparam name="TProvider">The provider type.</typeparam>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TIdentity">The identity type.</typeparam>
public class SignUpExternalAuthCode<TProvider, TUser, TIdentity> : BaseSignUpExternal<TProvider, TUser, TIdentity>
    where TProvider : ExternalLoginProviderAuthCode, new()
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>;