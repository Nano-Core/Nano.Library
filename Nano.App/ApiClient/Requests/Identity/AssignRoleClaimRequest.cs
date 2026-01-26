using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class AssignRoleClaimRequest : AssignRoleClaimRequest<Guid>;

/// <summary>
/// Request to assign a claim to a role.
/// </summary>
/// <typeparam name="TIdentity">Type of the role identifier.</typeparam>
public class AssignRoleClaimRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The role claim assignment information.
    /// </summary>
    public virtual AssignRoleClaim<TIdentity> AssignRoleClaim { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="AssignRoleClaimRequest{TIdentity}"/>.
    /// Sets the action to "roles/claims/assign".
    /// </summary>
    public AssignRoleClaimRequest()
    {
        this.Action = "roles/claims/assign";
    }

    /// <summary>
    /// Gets the request body containing the role claim assignment information.
    /// </summary>
    public override object GetBody()
    {
        return this.AssignRoleClaim;
    }
}