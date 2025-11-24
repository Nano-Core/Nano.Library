using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GenerateChangePhoneTokenRequest : GenerateChangePhoneTokenRequest<Guid>;

/// <inheritdoc />
public class GenerateChangePhoneTokenRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Change Phone Token.
    /// </summary>
    public virtual GenerateChangePhoneToken<TIdentity> ChangePhoneToken { get; set; } = new();

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