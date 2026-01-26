using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class AssignUserRoleRequest : AssignUserRoleRequest<Guid>;

/// <summary>
/// Request to assign a role to a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class AssignUserRoleRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The role assignment information.
    /// </summary>
    public virtual AssignUserRole<TIdentity> AssignUserRole { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="AssignUserRoleRequest{TIdentity}"/>.
    /// Sets the action to "roles/user/assign".
    /// </summary>
    public AssignUserRoleRequest()
    {
        this.Action = "roles/user/assign";
    }

    /// <summary>
    /// Gets the request body containing the role assignment information.
    /// </summary>
    public override object GetBody()
    {
        return this.AssignUserRole;
    }
}