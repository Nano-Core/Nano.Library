using Nano.Data.Abstractions.Identity.Models;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Request to create a new role.
/// </summary>
[PostAction(ActionRoutes.IDENTITY_ROLES_CREATE)]
public class CreateRoleRequest : BaseRequest
{
    /// <summary>
    /// The role information to create.
    /// </summary>
    [Required]
    [Body]
    public virtual CreateRole CreateRole { get; set; } = new();
}