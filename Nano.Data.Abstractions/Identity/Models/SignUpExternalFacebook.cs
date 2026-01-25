using System;
using Nano.Data.Abstractions.Entities.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Sign up a user using Facebook as an external login provider with implicit flow (default identity type is Guid).
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
public class SignUpExternalFacebook<TUser> : SignUpExternalImplicit<ExternalLoginProviderFacebook, TUser>
    where TUser : IEntityUser<Guid>, new();

/// <summary>
/// Sign up a user using Facebook as an external login provider with implicit flow.
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class SignUpExternalFacebook<TUser, TIdentity> : SignUpExternalImplicit<ExternalLoginProviderFacebook, TUser, TIdentity>
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>;