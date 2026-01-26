using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class RemoveUserRoleRequest : RemoveUserRoleRequest<Guid>;

/// <summary>
/// Represents a request to remove a role from a user.
/// </summary>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
public class RemoveUserRoleRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the user role removal information.
    /// </summary>
    public virtual RemoveUserRole<TIdentity> RemoveUserRole { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="RemoveUserRoleRequest{TIdentity}"/> with action set.
    /// </summary>
    public RemoveUserRoleRequest()
    {
        this.Action = "roles/user/remove";
    }

    /// <summary>
    /// Gets the body of the request containing the user role removal data.
    /// </summary>
    public override object GetBody()
    {
        return this.RemoveUserRole;
    }
}