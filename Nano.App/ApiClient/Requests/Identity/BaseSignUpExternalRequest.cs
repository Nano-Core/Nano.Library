using Nano.App.ApiClient.Models.Identity;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public abstract class BaseSignUpExternalRequest : BaseRequestPost;

/// <inheritdoc />
public abstract class BaseSignUpExternalRequest<TSignUp> : BaseSignUpExternalRequest
    where TSignUp : BaseSignUpExternal, new()
{
    /// <summary>
    /// Sign Up External.
    /// </summary>
    public virtual TSignUp SignUpExternal { get; set; } = new();

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.SignUpExternal;
    }
}