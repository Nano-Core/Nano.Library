using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to get external login data for Google.
/// </summary>
public class GetExternalLoginDataGoogleRequest : BaseGetExternalLoginDataRequest<ExternalLoginProviderGoogle>
{
    /// <summary>
    /// Initializes a new instance of <see cref="GetExternalLoginDataGoogleRequest"/> with action set.
    /// </summary>
    public GetExternalLoginDataGoogleRequest()
    {
        this.Action = "external/google/data";
    }
}