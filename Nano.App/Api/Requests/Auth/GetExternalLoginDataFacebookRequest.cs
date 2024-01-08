using Nano.Security.Models;

namespace Nano.App.Api.Requests.Auth;

/// <inheritdoc />
public class GetExternalLoginDataFacebookRequest : BaseGetExternalLoginDataRequest<LogInExternalProviderFacebook>
{
    /// <inheritdoc />
    public GetExternalLoginDataFacebookRequest()
    {
        this.Action = "external/facebook/data";
    }
}