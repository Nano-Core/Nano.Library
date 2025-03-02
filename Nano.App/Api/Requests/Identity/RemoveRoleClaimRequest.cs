using System;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class RemoveRoleClaimRequest : RemoveRoleClaimRequest<Guid>;

/// <inheritdoc />
public class RemoveRoleClaimRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Remove Role Claim.
    /// </summary>
    public virtual RemoveClaim<TIdentity> RemoveRoleClaim { get; set; } = new();

    /// <inheritdoc />
    public RemoveRoleClaimRequest()
    {
        this.Action = "roles/claims/remove";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.RemoveRoleClaim;
    }
}