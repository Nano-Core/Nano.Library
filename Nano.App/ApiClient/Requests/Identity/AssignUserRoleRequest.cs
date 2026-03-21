using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class AssignUserRoleRequest : AssignUserRoleRequest<Guid>;

/// <summary>
/// Request to assign a role to a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_ROLES_USER_ASSIGN)]
public class AssignUserRoleRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The role assignment information.
    /// </summary>
    [Required]
    [Body]
    public virtual AssignUserRole AssignUserRole { get; set; } = new();
}