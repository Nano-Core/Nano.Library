using System;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class RemoveExternalLoginRequest : RemoveExternalLoginRequest<Guid>;

/// <inheritdoc />
public class RemoveExternalLoginRequest<TIdentity> : BaseRequestDelete 
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// External Login
    /// </summary>
    public virtual RemoveExternalLogin<TIdentity> RemoveExternalLogin { get; set; }

    /// <inheritdoc />
    public RemoveExternalLoginRequest()
    {
        this.Action = "external-logins/remove";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return null;
    }
}