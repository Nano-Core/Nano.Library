using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Auth;

/// <inheritdoc />
public class GetExternalLoginDataGoogleRequest : BaseGetExternalLoginDataRequest<LogInExternalProviderGoogle>
{
    /// <inheritdoc />
    public GetExternalLoginDataGoogleRequest()
    {
        this.Action = "external/google/data";
    }
}