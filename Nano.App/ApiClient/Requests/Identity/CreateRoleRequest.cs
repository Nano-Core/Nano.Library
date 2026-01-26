using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Request to create a new role.
/// </summary>
public class CreateRoleRequest : BaseRequestPost
{
    /// <summary>
    /// The role information to create.
    /// </summary>
    public virtual CreateRole CreateRole { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="CreateRoleRequest"/>.
    /// Sets the action to "roles/create".
    /// </summary>
    public CreateRoleRequest()
    {
        this.Action = "roles/create";
    }

    /// <summary>
    /// Gets the request body containing the role to create.
    /// </summary>
    public override object GetBody()
    {
        return this.CreateRole;
    }
}