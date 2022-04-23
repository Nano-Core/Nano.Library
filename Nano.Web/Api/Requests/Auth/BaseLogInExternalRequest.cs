using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public abstract class BaseLogInExternalRequest : BaseRequestPost
    {

    }

    /// <inheritdoc />
    public abstract class BaseLogInExternalRequest<TLogin> : BaseLogInExternalRequest
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
}