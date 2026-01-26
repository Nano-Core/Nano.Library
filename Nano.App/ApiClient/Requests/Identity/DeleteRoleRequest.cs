using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Request to delete a role.
/// </summary>
public class DeleteRoleRequest : BaseRequestDelete
{
    /// <summary>
    /// The role information to delete.
    /// </summary>
    public virtual DeleteRole DeleteRole { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="DeleteRoleRequest"/>.
    /// Sets the action to "roles/delete".
    /// </summary>
    public DeleteRoleRequest()
    {
        this.Action = "roles/delete";
    }

    /// <summary>
    /// Gets the request body containing the role to delete.
    /// </summary>
    public override object GetBody()
    {
        return this.DeleteRole;
    }
}