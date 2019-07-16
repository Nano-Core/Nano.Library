using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public class LogInExternalRequest : BaseRequestPost
    {
        /// <summary>
        /// External Login.
        /// </summary>
        public virtual LoginExternal LoginExternal { get; set; }

        /// <inheritdoc />
        public LogInExternalRequest()
        {
            this.Action = "login/external";
            this.Controller = "auth";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.LoginExternal;
        }
    }
}