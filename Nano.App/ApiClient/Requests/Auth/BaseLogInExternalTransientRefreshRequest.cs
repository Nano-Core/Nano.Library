using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Base class for transient external login refresh requests. Carries no body - the access token being
/// refreshed is sent via the Authorization header (forwarded automatically by the api client), not the
/// request body, and the provider's own refresh token is recovered server-side from that same token.
/// </summary>
[PostAction(ActionRoutes.AUTH_LOGIN_EXTERNAL_TRANSIENT_REFRESH)]
public abstract class BaseLogInExternalTransientRefreshRequest(string providerName) : BaseLogInExternalRequest(providerName);
