using System;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class AssignRoleRequest : AssignRoleRequest<Guid>;

/// <inheritdoc />
public class AssignRoleRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Assign Role.
    /// </summary>
    public virtual AssignRole<TIdentity> AssignRole { get; set; } = new();

    /// <inheritdoc />
    public AssignRoleRequest()
    {
        this.Action = "roles/assign";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.AssignRole;
    }
}