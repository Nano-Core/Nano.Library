using Nano.Common.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <inheritdoc />
public class GetExternalLoginDataGoogleRequest : BaseGetExternalLoginDataRequest<ExternalLoginProviderGoogle>
{
    /// <inheritdoc />
    public GetExternalLoginDataGoogleRequest()
    {
        this.Action = "external/google/data";
    }
}