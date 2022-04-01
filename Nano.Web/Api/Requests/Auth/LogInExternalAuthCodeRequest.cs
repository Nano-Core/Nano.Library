using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public class LogInExternalAuthCodeRequest : BaseLogInExternalRequest<LoginExternalAuthCode>
    {
        /// <inheritdoc />
        public LogInExternalAuthCodeRequest()
        {
            this.Action = "login/external/authcode";
            this.Controller = "auth";
        }
    }
}