using Nano.App.ApiClient.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <inheritdoc />
public abstract class BaseGetExternalLoginDataRequest<TProvider> : BaseRequestPost
    where TProvider : BaseLogInExternalProvider, new()
{
    /// <summary>
    /// Provider.
    /// </summary>
    public virtual TProvider Provider { get; set; } = new();

    /// <inheritdoc />
    protected BaseGetExternalLoginDataRequest()
    {
        this.Controller = ControllerRoutes.AUTH_CONTROLLER_ROUTE;
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.Provider;
    }
}