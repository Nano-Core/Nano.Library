using System;
using Nano.App.ApiClient.Requests.Attributes;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetActiveRefreshTokensRequest : GetActiveRefreshTokensRequest<Guid>;

/// <summary>
/// Get Roles Request.
/// </summary>
public class GetActiveRefreshTokensRequest<TIdentity> : BaseRequestGet
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// Constructor.
    /// </summary>
    public GetActiveRefreshTokensRequest()
    {
        this.Action = "refresh-tokens/active";
    }
}