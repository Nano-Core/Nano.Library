using System;
using Nano.App.ApiClient.Models.Identity.External.Providers;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Models.Identity.External;

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