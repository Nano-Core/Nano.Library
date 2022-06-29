using System;
using Nano.Models.Interfaces;
using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity;

/// <inheritdoc />
public class SignUpExternalDirectRequest<TUser> : BaseSignUpExternalRequest<SignUpExternalFacebook<TUser, Guid>>
    where TUser : IEntityUser<Guid>, new()
{

}

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