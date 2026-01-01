using System;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class SignUpExternalFacebookRequest<TUser> : SignUpExternalFacebookRequest<TUser, Guid>
    where TUser : IEntityUser<Guid>, new();

/// <inheritdoc />
public class SignUpExternalFacebookRequest<TUser, TIdentity> : BaseSignUpExternalRequest<SignUpExternalFacebook<TUser, TIdentity>>
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public SignUpExternalFacebookRequest()
    {
        this.Action = "signup/external/facebook";
    }
}