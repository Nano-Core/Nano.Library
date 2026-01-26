using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public abstract class BaseSignUpExternalRequest : BaseRequestPost;

/// <summary>
/// Base request for signing up a user via an external provider.
/// </summary>
/// <typeparam name="TSignUp">Type of the sign-up information.</typeparam>
public abstract class BaseSignUpExternalRequest<TSignUp> : BaseSignUpExternalRequest
    where TSignUp : BaseSignUpExternal, new()
{
    /// <summary>
    /// The external sign-up information.
    /// </summary>
    public virtual TSignUp SignUpExternal { get; set; } = new();

    /// <summary>
    /// Gets the request body containing the external sign-up information.
    /// </summary>
    public override object GetBody()
    {
        return this.SignUpExternal;
    }
}