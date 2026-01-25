using System;
using Nano.Data.Abstractions.Entities.Abstractions;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class SignUpExternalDirectRequest<TUser> : SignUpExternalDirectRequest<TUser, Guid>
    where TUser : IEntityUser<Guid>, new();

/// <inheritdoc />
public class SignUpExternalDirectRequest<TUser, TIdentity> : BaseSignUpExternalRequest<SignUpExternalDirect<TUser, TIdentity>>
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public SignUpExternalDirectRequest()
    {
        this.Action = "signup/external/direct";
    }
}