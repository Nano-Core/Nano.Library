using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to retrieve all configured external login schemes.
/// </summary>
[GetAction(ActionRoutes.AUTH_EXTERNAL_SCHEMES)]
public class GetExternalSchemesRequest : BaseAuthRequest;