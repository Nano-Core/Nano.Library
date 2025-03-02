using System;
using Nano.Models.Interfaces;

namespace Nano.Security.Models;

/// <inheritdoc />
public class SignUpExternalGoogle<TUser> : SignUpExternalImplicit<ExternalLoginProviderGoogle, TUser>
    where TUser : IEntityUser<Guid>, new();

/// <summary>
/// Sign Up External Google.
/// </summary>
/// <typeparam name="TUser"></typeparam>
/// <typeparam name="TIdentity"></typeparam>
public class SignUpExternalGoogle<TUser, TIdentity> : SignUpExternalImplicit<ExternalLoginProviderGoogle, TUser, TIdentity>
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>;