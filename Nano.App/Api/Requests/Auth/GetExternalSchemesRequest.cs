using Nano.App.Consts;

namespace Nano.App.Api.Requests.Auth;

/// <inheritdoc />
public class GetExternalSchemesRequest : BaseRequestGet
{
    /// <inheritdoc />
    public GetExternalSchemesRequest()
    {
        this.Action = "external/schemes";
        this.Controller = Constants.AUTH_CONTROLLER_ROUTE;
    }
}