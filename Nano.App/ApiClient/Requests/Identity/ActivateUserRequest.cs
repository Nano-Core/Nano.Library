using System;
using Nano.App.ApiClient.Requests.Annotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ActivateUserRequest : ActivateUserRequest<Guid>;

/// <summary>
/// Request to activate a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class ActivateUserRequest<TIdentity> : BaseRequestPost
{
    /// <summary>
    /// The identifier of the user to activate.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity Id { get; set; } = default!;

    /// <summary>
    /// Initializes a new instance of <see cref="ActivateUserRequest{TIdentity}"/>.
    /// Sets the action to "activate".
    /// </summary>
    public ActivateUserRequest()
    {
        this.Action = "activate";
    }

    /// <summary>
    /// Gets the request body. Returns null for activation requests.
    /// </summary>
    public override object? GetBody()
    {
        return null;
    }
}