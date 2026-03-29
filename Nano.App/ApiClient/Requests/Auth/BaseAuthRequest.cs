using Nano.App.Consts;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a base auth request.
/// </summary>
public abstract class BaseAuthRequest : BaseRequest
{
    /// <summary>
    /// Initializes a new instance of <see cref="LogOutRequest"/> and sets the action and controller.
    /// </summary>
    protected BaseAuthRequest()
    {
        this.Controller = ControllerRoutes.AUTH;
    }
}