using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public class LogInExternalTransientImplicitRequest : BaseLogInExternalRequest<LoginExternalImplicit>
    {
        /// <inheritdoc />
        public LogInExternalTransientImplicitRequest()
        {
            this.Action = "login/external/implicit/transient";
            this.Controller = "auth";
        }
    }
}