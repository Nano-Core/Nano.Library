using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class RemoveRoleClaimRequest : RemoveRoleClaimRequest<Guid>;

/// <summary>
/// Represents a request to remove a role's claim.
/// </summary>
/// <typeparam name="TIdentity">The type of the role identifier.</typeparam>
public class RemoveRoleClaimRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the role claim removal information.
    /// </summary>
    public virtual RemoveUserClaim<TIdentity> RemoveRoleClaim { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="RemoveRoleClaimRequest{TIdentity}"/> with action set.
    /// </summary>
    public RemoveRoleClaimRequest()
    {
        this.Action = "roles/claims/remove";
    }

    /// <summary>
    /// Gets the body of the request containing the role claim removal data.
    /// </summary>
    public override object GetBody()
    {
        return this.RemoveRoleClaim;
    }
}