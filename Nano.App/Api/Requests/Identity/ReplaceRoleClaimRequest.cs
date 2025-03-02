using System;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class ReplaceRoleClaimRequest : ReplaceRoleClaimRequest<Guid>;

/// <inheritdoc />
public class ReplaceRoleClaimRequest<TIdentity> : BaseRequestPut
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Replace Role Claim.
    /// </summary>
    public virtual ReplaceRoleClaim<TIdentity> ReplaceRoleClaim { get; set; } = new();

    /// <inheritdoc />
    public ReplaceRoleClaimRequest()
    {
        this.Action = "roles/claims/replace";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.ReplaceRoleClaim;
    }
}