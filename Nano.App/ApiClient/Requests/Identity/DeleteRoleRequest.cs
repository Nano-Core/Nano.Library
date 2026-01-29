using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Request to delete a role.
/// </summary>
[DeleteAction(ActionRoutes.IDENTITY_ROLES_DELETE)]
public class DeleteRoleRequest : BaseRequest
{
    /// <summary>
    /// The role information to delete.
    /// </summary>
    [Required]
    [Body]
    public virtual DeleteRole DeleteRole { get; set; } = new();
}