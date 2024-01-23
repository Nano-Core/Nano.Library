using System;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class GenerateConfirmPhoneTokenRequest : GenerateConfirmPhoneTokenRequest<Guid>;

/// <inheritdoc />
public class GenerateConfirmPhoneTokenRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Confirm Phone Token.
    /// </summary>
    public virtual GenerateConfirmPhoneToken<TIdentity> ConfirmPhoneToken { get; set; } = new();

    /// <inheritdoc />
    public GenerateConfirmPhoneTokenRequest()
    {
        this.Action = "phone/confirm/token";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.ConfirmPhoneToken;
    }
}