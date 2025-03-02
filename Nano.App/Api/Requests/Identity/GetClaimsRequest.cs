using System;
using Nano.App.Api.Requests.Attributes;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class GetClaimsRequest : GetClaimsRequest<Guid>;

/// <summary>
/// Get User Claims Request.
/// </summary>
public class GetClaimsRequest<TIdentity> : BaseRequestGet
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public GetClaimsRequest()
    {
        this.Action = "claims";
    }
}