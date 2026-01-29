using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Request to add an external Microsoft login.
/// </summary>
[PostAction(ActionRoutes.IDENTITY_EXTERNAL_LOGINS_ADD_MICROSOFT)]
public class AddExternalLoginMicrosoftRequest : BaseAddExternalLoginRequest<LogInExternalMicrosoft>;