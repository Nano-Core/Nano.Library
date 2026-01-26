using System;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Sign up a user using Google as an external login provider with implicit flow (default identity type is Guid).
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
public class SignUpExternalGoogle<TUser> : SignUpExternalImplicit<ExternalLoginProviderGoogle, TUser>
    where TUser : IEntityUser<Guid>, new();

/// <summary>
/// Sign up a user using Google as an external login provider with implicit flow.
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class SignUpExternalGoogle<TUser, TIdentity> : SignUpExternalImplicit<ExternalLoginProviderGoogle, TUser, TIdentity>
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>;