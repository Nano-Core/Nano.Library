using System;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class GenerateChangePhoneTokenRequest : GenerateChangePhoneTokenRequest<Guid>;

/// <inheritdoc />
public class GenerateChangePhoneTokenRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Change Phone Token.
    /// </summary>
    public virtual GenerateResetPasswordToken<TIdentity> ChangePhoneToken { get; set; } = new();

    /// <inheritdoc />
    public GenerateChangePhoneTokenRequest()
    {
        this.Action = "phone/change/token";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.ChangePhoneToken;
    }
}