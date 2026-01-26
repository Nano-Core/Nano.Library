using Nano.App.ApiClient.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Base class for requests to retrieve external login provider data.
/// </summary>
/// <typeparam name="TProvider">The type of external login provider.</typeparam>
public abstract class BaseGetExternalLoginDataRequest<TProvider> : BaseRequestPost
    where TProvider : BaseLogInExternalProvider, new()
{
    /// <summary>
    /// The external login provider data.
    /// </summary>
    public virtual TProvider Provider { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="BaseGetExternalLoginDataRequest{TProvider}"/> and sets the controller.
    /// </summary>
    protected BaseGetExternalLoginDataRequest()
    {
        this.Controller = ControllerRoutes.AUTH_CONTROLLER_ROUTE;
    }

    /// <summary>
    /// Gets the body of the request containing the provider data.
    /// </summary>
    public override object GetBody()
    {
        return this.Provider;
    }
}