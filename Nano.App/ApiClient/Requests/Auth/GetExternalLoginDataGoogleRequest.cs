using Nano.App.ApiClient.Models.Identity.External.Providers;

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