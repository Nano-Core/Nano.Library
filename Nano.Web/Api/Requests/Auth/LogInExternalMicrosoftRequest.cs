using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public class LogInExternalMicrosoftRequest : BaseLogInExternalRequest<LoginExternalMicrosoft>
    {
        /// <inheritdoc />
        public LogInExternalMicrosoftRequest()
        {
            this.Action = "login/external/microsoft";
            this.Controller = "auth";
        }
    }
}