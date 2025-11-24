using Nano.App.ApiClient.Consts;

namespace Nano.App.ApiClient.Requests.Auth;

/// <inheritdoc />
public class GetExternalSchemesRequest : BaseRequestGet
{
    /// <inheritdoc />
    public GetExternalSchemesRequest()
    {
        this.Action = "external/schemes";
        this.Controller = ControllerRoutes.AUTH_CONTROLLER_ROUTE;
    }
}