using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Auth;

/// <inheritdoc />
public class GetExternalLoginDataMicrosoftRequest : BaseGetExternalLoginDataRequest<LogInExternalProviderMicrosoft>
{
    /// <inheritdoc />
    public GetExternalLoginDataMicrosoftRequest()
    {
        this.Action = "external/microsoft/data";
    }
}