using System;
using Nano.Models.Interfaces;
using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity;

/// <inheritdoc />
public class SignUpExternalMicrosoftRequest<TUser> : SignUpExternalMicrosoftRequest<TUser, Guid>
    where TUser : IEntityUser<Guid>, new()
{

}

/// <inheritdoc />
public class SignUpExternalMicrosoftRequest<TUser, TIdentity> : BaseSignUpExternalRequest<SignUpExternalMicrosoft<TUser, TIdentity>>
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public SignUpExternalMicrosoftRequest()
    {
        this.Action = "signup/external/microsoft";
    }
}