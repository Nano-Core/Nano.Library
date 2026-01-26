using System;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Sign up a user using an external login provider with authorization code flow (default identity type is Guid).
/// </summary>
/// <typeparam name="TProvider">The external login provider type.</typeparam>
/// <typeparam name="TUser">The user type.</typeparam>
public class SignUpExternalAuthCode<TProvider, TUser> : SignUpExternalAuthCode<TProvider, TUser, Guid>
    where TProvider : ExternalLoginProviderAuthCode, new()
    where TUser : IEntityUser<Guid>, new();

/// <summary>
/// Sign up a user using an external login provider with authorization code flow.
/// </summary>
/// <typeparam name="TProvider">The external login provider type.</typeparam>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class SignUpExternalAuthCode<TProvider, TUser, TIdentity> : BaseSignUpExternal<TProvider, TUser, TIdentity>
    where TProvider : ExternalLoginProviderAuthCode, new()
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>;
