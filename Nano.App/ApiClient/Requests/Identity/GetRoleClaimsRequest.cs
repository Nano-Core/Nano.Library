using System;
using Nano.App.ApiClient.Requests.Annotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetRoleClaimsRequest : GetRoleClaimsRequest<Guid>;

/// <summary>
/// Represents a request to retrieve claims assigned to a specific role.
/// </summary>
/// <typeparam name="TIdentity">The type of the role identifier.</typeparam>
public class GetRoleClaimsRequest<TIdentity> : BaseRequestGet
{
    /// <summary>
    /// The role identifier.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity RoleId { get; set; } = default!;

    /// <summary>
    /// Initializes a new instance of <see cref="GetRoleClaimsRequest{TIdentity}"/> with action set.
    /// </summary>
    public GetRoleClaimsRequest()
    {
        this.Action = "roles/claims";
    }
}