using Nano.App.ApiClient.Requests.Auth;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public abstract class BaseAddExternalLoginRequest : BaseRequestPost;

/// <summary>
/// Base request for adding an external login.
/// </summary>
/// <typeparam name="TLogin">Type of the external login information.</typeparam>
public abstract class BaseAddExternalLoginRequest<TLogin> : BaseLogInExternalRequest
    where TLogin : LogInExternal, new()
{
    /// <summary>
    /// The external login information.
    /// </summary>
    public virtual TLogin LoginExternal { get; set; } = new();

    /// <summary>
    /// Gets the request body containing the external login information.
    /// </summary>
    public override object GetBody()
    {
        return this.LoginExternal;
    }
}