using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class AssignUserRoleRequest : AssignUserRoleRequest<Guid>;

/// <inheritdoc />
public class AssignUserRoleRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Assign Role.
    /// </summary>
    public virtual AssignUserRole<TIdentity> AssignUserRole { get; set; } = new();

    /// <inheritdoc />
    public AssignUserRoleRequest()
    {
        this.Action = "roles/user/assign";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.AssignUserRole;
    }
}