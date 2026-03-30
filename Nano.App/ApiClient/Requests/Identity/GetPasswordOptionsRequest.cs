using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Represents a request to retrieve password policy and options.
/// </summary>
[GetAction(ActionRoutes.IDENTITY_PASSWORD_OPTIONS)]
public class GetPasswordOptionsRequest : BaseRequest;