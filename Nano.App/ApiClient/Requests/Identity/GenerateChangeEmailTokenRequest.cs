using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GenerateChangeEmailTokenRequest : GenerateChangeEmailTokenRequest<Guid>;

/// <inheritdoc />
public class GenerateChangeEmailTokenRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Change Email Token.
    /// </summary>
    public virtual GenerateChangeEmailToken<TIdentity> ChangeEmailToken { get; set; } = new();

    /// <inheritdoc />
    public GenerateChangeEmailTokenRequest()
    {
        this.Action = "email/change/token";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.ChangeEmailToken;
    }
}