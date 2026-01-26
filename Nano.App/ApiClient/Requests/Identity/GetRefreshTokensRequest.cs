using System;
using Nano.App.ApiClient.Requests.Annotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetRefreshTokensRequest : GetRefreshTokensRequest<Guid>;

/// <summary>
/// Represents a request to retrieve refresh tokens for a specific user.
/// </summary>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
public class GetRefreshTokensRequest<TIdentity> : BaseRequestGet
{
    /// <summary>
    /// The user identifier.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// Initializes a new instance of <see cref="GetRefreshTokensRequest{TIdentity}"/> with action set.
    /// </summary>
    public GetRefreshTokensRequest()
    {
        this.Action = "refresh-tokens";
    }
}