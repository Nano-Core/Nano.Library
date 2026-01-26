using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to get external login data for Facebook.
/// </summary>
public class GetExternalLoginDataFacebookRequest : BaseGetExternalLoginDataRequest<ExternalLoginProviderFacebook>
{
    /// <summary>
    /// Initializes a new instance of <see cref="GetExternalLoginDataFacebookRequest"/> with action set.
    /// </summary>
    public GetExternalLoginDataFacebookRequest()
    {
        this.Action = "external/facebook/data";
    }
}