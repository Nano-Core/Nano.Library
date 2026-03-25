using Nano.Data.Abstractions.Identity.Authentication.Models;
using System;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Sign up a user using Microsoft as an external login provider with authorization code flow (default identity type is Guid).
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
public class SignUpExternalMicrosoft<TUser> : SignUpExternalMicrosoft<TUser, Guid>
    where TUser : IEntityUser<Guid>, new();

/// <summary>
/// Sign up a user using Microsoft as an external login provider with authorization code flow.
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class SignUpExternalMicrosoft<TUser, TIdentity> : BaseSignUpExternal<ExternalProviderMicrosoft, AuthCodeFlow, TUser, TIdentity>
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>;