using System;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class SignUpExternalGoogleRequest<TUser> : SignUpExternalGoogleRequest<TUser, Guid>
    where TUser : IEntityUser<Guid>, new();

/// <summary>
/// Represents a request to sign up a user using an external Google account.
/// </summary>
/// <typeparam name="TUser">The type of the user entity.</typeparam>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
public class SignUpExternalGoogleRequest<TUser, TIdentity> : BaseSignUpExternalRequest<SignUpExternalGoogle<TUser, TIdentity>>
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Initializes a new instance of <see cref="SignUpExternalGoogleRequest{TUser, TIdentity}"/> with action set.
    /// </summary>
    public SignUpExternalGoogleRequest()
    {
        this.Action = "signup/external/google";
    }
}