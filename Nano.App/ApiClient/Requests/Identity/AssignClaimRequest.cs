using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class AssignClaimRequest : AssignClaimRequest<Guid>;

/// <inheritdoc />
public class AssignClaimRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Assign Claim.
    /// </summary>
    public virtual AssignClaim<TIdentity> AssignClaim { get; set; } = new();

    /// <inheritdoc />
    public AssignClaimRequest()
    {
        this.Action = "claims/assign";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.AssignClaim;
    }
}