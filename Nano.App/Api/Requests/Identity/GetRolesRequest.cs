using System;
using Nano.App.Api.Requests.Attributes;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class GetRolesRequest : GetRolesRequest<Guid>;

/// <summary>
/// Get Roles Request.
/// </summary>
public class GetRolesRequest<TIdentity> : BaseRequestGet
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public GetRolesRequest()
    {
        this.Action = "roles";
    }
}