using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class SetUsernameRequest : SetUsernameRequest<Guid>;

/// <summary>
/// Represents a request to set a user's username.
/// </summary>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
public class SetUsernameRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the username information to set.
    /// </summary>
    public virtual SetUsername<TIdentity> SetUsername { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="SetUsernameRequest{TIdentity}"/> with action set.
    /// </summary>
    public SetUsernameRequest()
    {
        this.Action = "username/set";
    }

    /// <summary>
    /// Gets the body of the request containing the username data.
    /// </summary>
    public override object GetBody()
    {
        return this.SetUsername;
    }
}