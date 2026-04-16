using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetUserRolesRequest : GetUserRolesRequest<Guid>;

/// <summary>
/// Represents a request to retrieve the roles assigned to a specific user.
/// </summary>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
[GetAction(ActionRoutes.IDENTITY_USER_ROLES)]
public class GetUserRolesRequest<TIdentity> : BaseRequest
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity Id { get; set; }
}