using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class AssignApiKeyRoleRequest : AssignApiKeyRoleRequest<Guid>;

/// <summary>
/// Request to assign a role to a api-key.
/// </summary>
/// <typeparam name="TIdentity">Type of the api-key identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_API_KEY_ROLES_ASSIGN)]
public class AssignApiKeyRoleRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the api-key.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity ApiKeyId { get; set; }

    /// <summary>
    /// The role assignment information.
    /// </summary>
    [Required]
    [Body]
    public virtual required AssignRole AssignRole { get; set; }
}