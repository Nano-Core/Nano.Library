using System;
using Nano.App.ApiClient.Requests.Annotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetActiveRefreshTokensRequest : GetActiveRefreshTokensRequest<Guid>;

/// <summary>
/// Request to retrieve active refresh tokens for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class GetActiveRefreshTokensRequest<TIdentity> : BaseRequestGet
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// Initializes a new instance of <see cref="GetActiveRefreshTokensRequest{TIdentity}"/>.
    /// Sets the action to "refresh-tokens/active".
    /// </summary>
    public GetActiveRefreshTokensRequest()
    {
        this.Action = "refresh-tokens/active";
    }
}