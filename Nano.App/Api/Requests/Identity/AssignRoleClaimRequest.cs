using System;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class AssignRoleClaimRequest : AssignRoleClaimRequest<Guid>;

/// <inheritdoc />
public class AssignRoleClaimRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Assign Role Claim.
    /// </summary>
    public virtual AssignRoleClaim<TIdentity> AssignRoleClaim { get; set; } = new();

    /// <inheritdoc />
    public AssignRoleClaimRequest()
    {
        this.Action = "roles/claims/assign";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.AssignRoleClaim;
    }
}