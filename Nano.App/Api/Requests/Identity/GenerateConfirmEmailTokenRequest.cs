using Nano.Security.Models;
using System;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class GenerateConfirmEmailTokenRequest : GenerateConfirmEmailTokenRequest<Guid>;

/// <inheritdoc />
public class GenerateConfirmEmailTokenRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Confirm Email Token.
    /// </summary>
    public virtual GenerateConfirmEmailToken<TIdentity> ConfirmEmailToken { get; set; } = new();

    /// <inheritdoc />
    public GenerateConfirmEmailTokenRequest()
    {
        this.Action = "email/confirm/token";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.ConfirmEmailToken;
    }
}