using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public class LogInExternalImplicitRequest : BaseLogInExternalRequest<LoginExternalImplicit>
    {
        /// <inheritdoc />
        public LogInExternalImplicitRequest()
        {
            this.Action = "login/external/implicit";
            this.Controller = "auth";
        }
    }
}