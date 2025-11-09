using System;
using Nano.Models.Interfaces;

namespace Nano.Security.Models;

/// <inheritdoc />
public class SignUpExternalFacebook<TUser> : SignUpExternalImplicit<ExternalLoginProviderFacebook, TUser>
    where TUser : IEntityUser<Guid>, new();

/// <summary>
/// Sign Up External Facebook.
/// </summary>
/// <typeparam name="TUser"></typeparam>
/// <typeparam name="TIdentity"></typeparam>
public class SignUpExternalFacebook<TUser, TIdentity> : SignUpExternalImplicit<ExternalLoginProviderFacebook, TUser, TIdentity>
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>;