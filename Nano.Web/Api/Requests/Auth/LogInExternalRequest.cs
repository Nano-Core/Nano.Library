using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public class LogInExternalRequest : BaseRequestPost
    {
        /// <summary>
        /// Login External.
        /// </summary>
        public virtual LoginExternal LoginExternal { get; set; }

        /// <inheritdoc />
        public LogInExternalRequest()
        {
            this.Action = "external/login";
            this.Controller = "auth";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.LoginExternal;
        }
    }
}