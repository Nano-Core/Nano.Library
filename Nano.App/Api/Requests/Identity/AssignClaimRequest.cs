using System;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

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
        this.Action = "claim/assign";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.AssignClaim;
    }
}