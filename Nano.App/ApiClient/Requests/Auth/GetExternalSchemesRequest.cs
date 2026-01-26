using Nano.App.ApiClient.Consts;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to retrieve all configured external login schemes.
/// </summary>
public class GetExternalSchemesRequest : BaseRequestGet
{
    /// <summary>
    /// Initializes a new instance of <see cref="GetExternalSchemesRequest"/> with action and controller set.
    /// </summary>
    public GetExternalSchemesRequest()
    {
        this.Action = "external/schemes";
        this.Controller = ControllerRoutes.AUTH_CONTROLLER_ROUTE;
    }
}