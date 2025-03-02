using Nano.App.Api.Requests.Auth;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

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