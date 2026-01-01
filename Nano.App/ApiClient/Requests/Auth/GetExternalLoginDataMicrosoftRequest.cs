using Nano.Common.Identity.Authentication.Models;

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