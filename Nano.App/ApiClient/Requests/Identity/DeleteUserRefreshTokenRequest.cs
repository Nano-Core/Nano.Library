using System;
using Nano.App.ApiClient.Requests.Annotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class DeleteUserRefreshTokenRequest : DeleteUserRefreshTokenRequest<Guid>;

/// <summary>
/// Request to delete a user's refresh token.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class DeleteUserRefreshTokenRequest<TIdentity> : BaseRequestDelete
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the refresh token to delete.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity RefreshTokenId { get; set; } = default!;

    /// <summary>
    /// Initializes a new instance of <see cref="DeleteUserRefreshTokenRequest{TIdentity}"/>.
    /// Sets the action to "refresh-tokens/delete".
    /// </summary>
    public DeleteUserRefreshTokenRequest()
    {
        this.Action = "refresh-tokens/delete";
    }

    /// <summary>
    /// Gets the request body. Always returns null for delete requests.
    /// </summary>
    public override object? GetBody()
    {
        return null;
    }
}