using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to get external login data for Google.
/// </summary>
[PostAction(ActionRoutes.AUTH_EXTERNAL_GOOGLE_DATA)]
public class GetExternalLoginDataGoogleRequest : BaseGetExternalLoginDataRequest<ExternalLoginProviderGoogle>;