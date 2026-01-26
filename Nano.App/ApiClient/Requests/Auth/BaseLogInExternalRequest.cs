using Nano.App.ApiClient.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <inheritdoc />
public abstract class BaseLogInExternalRequest : BaseRequestPost;

/// <summary>
/// Base class for external login requests.
/// </summary>
/// <typeparam name="TLogin">The type of external login data.</typeparam>
public abstract class BaseLogInExternalRequest<TLogin> : BaseLogInExternalRequest
    where TLogin : LogInExternal, new()
{
    /// <summary>
    /// Contains the external login data for the request.
    /// </summary>
    public virtual TLogin LoginExternal { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="BaseLogInExternalRequest{TLogin}"/> with action and controller set.
    /// </summary>
    protected BaseLogInExternalRequest()
    {
        this.Controller = ControllerRoutes.AUTH_CONTROLLER_ROUTE;
    }

    /// <summary>
    /// Gets the body of the request containing the external login data.
    /// </summary>
    public override object GetBody()
    {
        return this.LoginExternal;
    }
}