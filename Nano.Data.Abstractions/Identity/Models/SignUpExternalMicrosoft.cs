using System;
using Nano.Common.Identity.Authentication.Models;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class SignUpExternalMicrosoft<TUser> : SignUpExternalAuthCode<ExternalLoginProviderMicrosoft, TUser>
    where TUser : IEntityUser<Guid>, new();

/// <summary>
/// Sign Up External Microsoft.
/// </summary>
/// <typeparam name="TUser"></typeparam>
/// <typeparam name="TIdentity"></typeparam>
public class SignUpExternalMicrosoft<TUser, TIdentity> : SignUpExternalAuthCode<ExternalLoginProviderMicrosoft, TUser, TIdentity>
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>;