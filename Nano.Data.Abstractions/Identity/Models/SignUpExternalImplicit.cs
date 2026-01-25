using System;
using Nano.Data.Abstractions.Entities.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Sign up a user using an external login provider with implicit flow (default identity type is Guid).
/// </summary>
/// <typeparam name="TProvider">The external login provider type.</typeparam>
/// <typeparam name="TUser">The user type.</typeparam>
public class SignUpExternalImplicit<TProvider, TUser> : SignUpExternalImplicit<TProvider, TUser, Guid>
    where TProvider : ExternalLoginProviderImplicit, new()
    where TUser : IEntityUser<Guid>, new();

/// <summary>
/// Sign up a user using an external login provider with implicit flow.
/// </summary>
/// <typeparam name="TProvider">The external login provider type.</typeparam>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class SignUpExternalImplicit<TProvider, TUser, TIdentity> : BaseSignUpExternal<TProvider, TUser, TIdentity>
    where TProvider : ExternalLoginProviderImplicit, new()
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>;