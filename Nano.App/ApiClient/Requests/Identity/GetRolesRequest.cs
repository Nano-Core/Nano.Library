namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Get Roles Request.
/// </summary>
public class GetRolesRequest : BaseRequestGet
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public GetRolesRequest()
    {
        this.Action = "roles";
    }
}