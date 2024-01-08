namespace Nano.App.Api.Requests.Auth;

/// <inheritdoc />
public class GetExternalSchemesRequest : BaseRequestGet
{
    /// <inheritdoc />
    public GetExternalSchemesRequest()
    {
        this.Action = "external/schemes";
        this.Controller = "auth";
    }
}