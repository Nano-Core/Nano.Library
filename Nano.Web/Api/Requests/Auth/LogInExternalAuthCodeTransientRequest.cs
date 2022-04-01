using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public class LogInExternalAuthCodeTransientRequest : BaseLogInExternalRequest<LoginExternalAuthCode>
    {
        /// <inheritdoc />
        public LogInExternalAuthCodeTransientRequest()
        {
            this.Action = "login/external/authcode/transient";
            this.Controller = "auth";
        }
    }
}