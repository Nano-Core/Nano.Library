using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class DeleteRoleRequest : BaseRequestDelete
{
    /// <summary>
    /// Delete Role.
    /// </summary>
    public virtual DeleteRole DeleteRole { get; set; } = new();

    /// <inheritdoc />
    public DeleteRoleRequest()
    {
        this.Action = "roles/delete";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.DeleteRole;
    }
}