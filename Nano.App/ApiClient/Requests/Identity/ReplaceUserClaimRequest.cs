using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ReplaceUserClaimRequest : ReplaceUserClaimRequest<Guid>;

/// <inheritdoc />
public class ReplaceUserClaimRequest<TIdentity> : BaseRequestPut
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Replace Claim.
    /// </summary>
    public virtual ReplaceUserClaim<TIdentity> ReplaceUserClaim { get; set; } = new();

    /// <inheritdoc />
    public ReplaceUserClaimRequest()
    {
        this.Action = "claims/replace";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.ReplaceUserClaim;
    }
}