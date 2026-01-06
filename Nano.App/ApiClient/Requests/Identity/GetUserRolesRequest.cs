using System;
using Nano.App.ApiClient.Requests.Attributes;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetUserRolesRequest : GetUserRolesRequest<Guid>;

/// <summary>
/// Get User Roles Request.
/// </summary>
public class GetUserRolesRequest<TIdentity> : BaseRequestGet
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public GetUserRolesRequest()
    {
        this.Action = "roles";
    }
}