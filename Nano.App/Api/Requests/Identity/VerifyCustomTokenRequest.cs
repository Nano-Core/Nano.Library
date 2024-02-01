using System;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class VerifyCustomTokenRequest : VerifyCustomTokenRequest<Guid>;

/// <inheritdoc />
public class VerifyCustomTokenRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Confirm Email.
    /// </summary>
    public virtual CustomPurposeToken<TIdentity> CustomPurposeToken { get; set; } = new();

    /// <inheritdoc />
    public VerifyCustomTokenRequest()
    {
        this.Action = "token/custom/verify";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.CustomPurposeToken;
    }
}