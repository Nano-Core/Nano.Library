using Nano.App.Api.Requests.Attributes;
using System;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class GetRoleClaimsRequest : GetRoleClaimsRequest<Guid>;

/// <summary>
/// Get Role Claims Request.
/// </summary>
public class GetRoleClaimsRequest<TIdentity> : BaseRequestGet
{
    /// <summary>
    /// Role Id.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity RoleId { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public GetRoleClaimsRequest()
    {
        this.Action = "roles/claims";
    }
}