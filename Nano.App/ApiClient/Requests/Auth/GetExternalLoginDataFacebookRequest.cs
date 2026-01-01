using Nano.Common.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <inheritdoc />
public class GetExternalLoginDataFacebookRequest : BaseGetExternalLoginDataRequest<ExternalLoginProviderFacebook>
{
    /// <inheritdoc />
    public GetExternalLoginDataFacebookRequest()
    {
        this.Action = "external/facebook/data";
    }
}