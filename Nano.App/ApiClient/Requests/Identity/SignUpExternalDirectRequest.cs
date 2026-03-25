using System;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class SignUpExternalDirectRequest<TUser> : SignUpExternalDirectRequest<TUser, Guid>
    where TUser : IEntityUser<Guid>, new();

/// <summary>
/// Represents a request to sign up a user using an external direct account.
/// </summary>
/// <typeparam name="TUser">The type of the user entity.</typeparam>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_SIGNUP_EXTERNAL_DIRECT)]
public class SignUpExternalDirectRequest<TUser, TIdentity> : BaseSignUpExternalRequest<SignUpExternalDirect<TUser, TIdentity>, TUser, TIdentity>
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>;