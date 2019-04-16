using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public class LogInExternalRequest : BaseRequestPost
    {
        /// <summary>
        /// Login.
        /// </summary>
        public virtual LoginExternal LoginExternal { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
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