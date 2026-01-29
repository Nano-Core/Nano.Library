using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Represents a request to retrieve all available roles.
/// </summary>
[GetAction(ActionRoutes.IDENTITY_ROLES)]
public class GetRolesRequest : BaseRequest;