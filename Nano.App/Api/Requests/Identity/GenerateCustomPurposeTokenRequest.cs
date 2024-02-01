using System;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class GenerateCustomPurposeTokenRequest : GenerateCustomPurposeTokenRequest<Guid>;

/// <inheritdoc />
public class GenerateCustomPurposeTokenRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Custom Purpose Token.
    /// </summary>
    public virtual GenerateCustomPurposeToken<TIdentity> CustomPurposeToken { get; set; } = new();

    /// <inheritdoc />
    public GenerateCustomPurposeTokenRequest()
    {
        this.Action = "token/custom";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.CustomPurposeToken;
    }
}