using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to log out the current user.
/// </summary>
[PostAction(ActionRoutes.AUTH_LOGOUT)]
public class LogOutRequest : BaseAuthRequest;