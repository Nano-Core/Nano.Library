using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetRoleClaimsRequest : GetRoleClaimsRequest<Guid>;

/// <summary>
/// Represents a request to retrieve claims assigned to a specific role.
/// </summary>
/// <typeparam name="TIdentity">The type of the role identifier.</typeparam>
[GetAction(ActionRoutes.IDENTITY_ROLES_CLAIMS)]
public class GetRoleClaimsRequest<TIdentity> : BaseRequest
{
    /// <summary>
    /// The role identifier.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity RoleId { get; set; }
}