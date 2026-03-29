using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class RemoveUserRoleRequest : RemoveUserRoleRequest<Guid>;

/// <summary>
/// Represents a request to remove a role from a user.
/// </summary>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
[DeleteAction(ActionRoutes.IDENTITY_ROLES_USER_REMOVE)]
public class RemoveUserRoleRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the user role removal information.
    /// </summary>
    [Required]
    [Body]
    public virtual RemoveUserRole RemoveUserRole { get; set; } = new();
}