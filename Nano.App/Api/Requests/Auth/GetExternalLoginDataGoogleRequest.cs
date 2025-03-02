using Nano.Security.Models;

namespace Nano.App.Api.Requests.Auth;

/// <inheritdoc />
public class GetExternalLoginDataGoogleRequest : BaseGetExternalLoginDataRequest<ExternalLoginProviderGoogle>
{
    /// <inheritdoc />
    public GetExternalLoginDataGoogleRequest()
    {
        this.Action = "external/google/data";
    }
}