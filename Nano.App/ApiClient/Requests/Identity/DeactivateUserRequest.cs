using System;
using Nano.App.ApiClient.Requests.Annotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class DeactivateUserRequest : DeactivateUserRequest<Guid>;

/// <summary>
/// Request to deactivate a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class DeactivateUserRequest<TIdentity> : BaseRequestDelete
{
    /// <summary>
    /// The identifier of the user to deactivate.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity Id { get; set; } = default!;

    /// <summary>
    /// Initializes a new instance of <see cref="DeactivateUserRequest{TIdentity}"/>.
    /// Sets the action to "activate".
    /// </summary>
    public DeactivateUserRequest()
    {
        this.Action = "activate";
    }

    /// <summary>
    /// Gets the request body. Always returns null for delete requests.
    /// </summary>
    public override object? GetBody()
    {
        return null;
    }
}