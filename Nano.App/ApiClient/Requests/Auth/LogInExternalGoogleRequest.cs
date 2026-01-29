using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to log in using an external Google account.
/// </summary>
[PostAction(ActionRoutes.AUTH_LOGIN_EXTERNAL_GOOGLE)]
public class LogInExternalGoogleRequest : BaseLogInExternalRequest<LogInExternalGoogle>;