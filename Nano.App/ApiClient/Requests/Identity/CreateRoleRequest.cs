using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class CreateRoleRequest : BaseRequestPost
{
    /// <summary>
    /// Create Role.
    /// </summary>
    public virtual CreateRole CreateRole { get; set; } = new();

    /// <inheritdoc />
    public CreateRoleRequest()
    {
        this.Action = "roles/create";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.CreateRole;
    }
}