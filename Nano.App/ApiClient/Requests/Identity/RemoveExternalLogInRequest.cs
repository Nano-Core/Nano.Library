using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class RemoveExternalLoginRequest : RemoveExternalLoginRequest<Guid>;

/// <summary>
/// Represents a request to remove an external login from a user.
/// </summary>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
public class RemoveExternalLoginRequest<TIdentity> : BaseRequestDelete
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the external login removal information.
    /// </summary>
    public virtual RemoveExternalLogin<TIdentity> RemoveExternalLogin { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="RemoveExternalLoginRequest{TIdentity}"/> with action set.
    /// </summary>
    public RemoveExternalLoginRequest()
    {
        this.Action = "external-logins/remove";
    }

    /// <summary>
    /// Gets the body of the request containing the external login removal data.
    /// </summary>
    public override object GetBody()
    {
        return this.RemoveExternalLogin;
    }
}