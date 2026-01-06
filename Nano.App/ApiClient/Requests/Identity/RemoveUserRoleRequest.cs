using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class RemoveUserRoleRequest : RemoveUserRoleRequest<Guid>;

/// <inheritdoc />
public class RemoveUserRoleRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Assign Role.
    /// </summary>
    public virtual RemoveUserRole<TIdentity> RemoveUserRole { get; set; } = new();

    /// <inheritdoc />
    public RemoveUserRoleRequest()
    {
        this.Action = "roles/user/remove";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.RemoveUserRole;
    }
}