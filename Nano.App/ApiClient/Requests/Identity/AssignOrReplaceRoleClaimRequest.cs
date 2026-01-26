using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class AssignOrReplaceRoleClaimRequest : AssignOrReplaceRoleClaimRequest<Guid>;

/// <summary>
/// Request to assign or replace a claim for a role.
/// </summary>
/// <typeparam name="TIdentity">Type of the role identifier.</typeparam>
public class AssignOrReplaceRoleClaimRequest<TIdentity> : BaseRequestPut
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The role claim assignment or replacement information.
    /// </summary>
    public virtual AssignOrReplaceRoleClaim<TIdentity> AssignOrReplaceRoleClaim { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="AssignOrReplaceRoleClaimRequest{TIdentity}"/>.
    /// Sets the action to "roles/claims/assign-or-replace".
    /// </summary>
    public AssignOrReplaceRoleClaimRequest()
    {
        this.Action = "roles/claims/assign-or-replace";
    }

    /// <summary>
    /// Gets the request body containing the role claim assignment or replacement information.
    /// </summary>
    public override object GetBody()
    {
        return this.AssignOrReplaceRoleClaim;
    }
}