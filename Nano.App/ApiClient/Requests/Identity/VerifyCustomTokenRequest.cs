using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class VerifyCustomTokenRequest : VerifyCustomTokenRequest<Guid>;

/// <inheritdoc />
public class VerifyCustomTokenRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Custom Purpose Token.
    /// </summary>
    public virtual ConfirmCustomPurpose<TIdentity> ConfirmCustomPurpose { get; set; } = new();

    /// <inheritdoc />
    public VerifyCustomTokenRequest()
    {
        this.Action = "token/custom-purpose/confirm";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.ConfirmCustomPurpose;
    }
}