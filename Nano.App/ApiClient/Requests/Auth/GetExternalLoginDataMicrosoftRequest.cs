using Nano.App.ApiClient.Models.Identity.External.Providers;

namespace Nano.App.ApiClient.Requests.Auth;

/// <inheritdoc />
public class GetExternalLoginDataMicrosoftRequest : BaseGetExternalLoginDataRequest<ExternalLoginProviderMicrosoft>
{
    /// <inheritdoc />
    public GetExternalLoginDataMicrosoftRequest()
    {
        this.Action = "external/microsoft/data";
    }
}