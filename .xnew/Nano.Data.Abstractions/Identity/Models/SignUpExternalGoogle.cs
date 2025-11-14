using System;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Identity.Models;

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