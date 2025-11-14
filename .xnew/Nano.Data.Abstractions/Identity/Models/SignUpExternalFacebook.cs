using System;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Identity.Models;

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