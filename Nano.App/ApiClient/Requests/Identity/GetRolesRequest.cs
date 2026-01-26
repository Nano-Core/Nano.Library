namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Represents a request to retrieve all available roles.
/// </summary>
public class GetRolesRequest : BaseRequestGet
{
    /// <summary>
    /// Initializes a new instance of <see cref="GetRolesRequest"/> with action set.
    /// </summary>
    public GetRolesRequest()
    {
        this.Action = "roles";
    }
}