using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class AssignUserClaimRequest : AssignUserClaimRequest<Guid>;

/// <inheritdoc />
public class AssignUserClaimRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Assign Claim.
    /// </summary>
    public virtual AssignUserClaim<TIdentity> AssignUserClaim { get; set; } = new();

    /// <inheritdoc />
    public AssignUserClaimRequest()
    {
        this.Action = "claims/assign";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.AssignUserClaim;
    }
}