using Nano.Security.Models;

namespace Nano.App.Api.Requests.Auth;

/// <inheritdoc />
public class GetExternalLoginDataFacebookRequest : BaseGetExternalLoginDataRequest<ExternalLoginProviderFacebook>
{
    /// <inheritdoc />
    public GetExternalLoginDataFacebookRequest()
    {
        this.Action = "external/facebook/data";
    }
}