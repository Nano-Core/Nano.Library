using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public class LogInRequest : BaseRequestPost
    {
        /// <summary>
        /// Login.
        /// </summary>
        public virtual Login Login { get; set; }

        /// <inheritdoc />
        public LogInRequest()
        {
            this.Action = "login";
            this.Controller = "auth";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.Login;
        }
    }
}