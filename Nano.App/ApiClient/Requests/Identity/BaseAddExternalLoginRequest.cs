using Nano.App.ApiClient.Requests.Auth;
using Nano.Common.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public abstract class BaseAddExternalLoginRequest : BaseRequestPost;

/// <inheritdoc />
public abstract class BaseAddExternalLoginRequest<TLogin> : BaseLogInExternalRequest
    where TLogin : LogInExternal, new()
{
    /// <summary>
    /// LogIn External.
    /// </summary>
    public virtual TLogin LoginExternal { get; set; } = new();

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.LoginExternal;
    }
}