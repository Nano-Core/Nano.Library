using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ReplaceClaimRequest : ReplaceClaimRequest<Guid>;

/// <inheritdoc />
public class ReplaceClaimRequest<TIdentity> : BaseRequestPut
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Replace Claim.
    /// </summary>
    public virtual ReplaceClaim<TIdentity> ReplaceClaim { get; set; } = new();

    /// <inheritdoc />
    public ReplaceClaimRequest()
    {
        this.Action = "claims/replace";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.ReplaceClaim;
    }
}