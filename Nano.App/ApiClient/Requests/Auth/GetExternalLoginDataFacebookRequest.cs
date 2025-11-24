using Nano.App.ApiClient.Models.Identity.External.Providers;

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