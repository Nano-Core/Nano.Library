using System;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class RemoveRoleRequest : RemoveRoleRequest<Guid>;

/// <inheritdoc />
public class RemoveRoleRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Assign Role.
    /// </summary>
    public virtual RemoveRole<TIdentity> RemoveRole { get; set; } = new();

    /// <inheritdoc />
    public RemoveRoleRequest()
    {
        this.Action = "roles/remove";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.RemoveRole;
    }
}