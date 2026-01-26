using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to get external login data for Microsoft.
/// </summary>
public class GetExternalLoginDataMicrosoftRequest : BaseGetExternalLoginDataRequest<ExternalLoginProviderMicrosoft>
{
    /// <summary>
    /// Initializes a new instance of <see cref="GetExternalLoginDataMicrosoftRequest"/> with action set.
    /// </summary>
    public GetExternalLoginDataMicrosoftRequest()
    {
        this.Action = "external/microsoft/data";
    }
}