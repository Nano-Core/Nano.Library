using System;
using Nano.App.ApiClient.Requests.Attributes;

namespace Nano.App.ApiClient.Requests.Identity;

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
    public virtual TIdentity RoleId { get; set; } = default!;

    /// <summary>
    /// Constructor.
    /// </summary>
    public GetRoleClaimsRequest()
    {
        this.Action = "roles/claims";
    }
}