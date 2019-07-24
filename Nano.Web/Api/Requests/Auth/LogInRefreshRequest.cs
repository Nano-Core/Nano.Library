using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public class LogInRefreshRequest : BaseRequestPost
    {
        /// <summary>
        /// Login.
        /// </summary>
        public virtual LoginRefresh LoginRefresh { get; set; }

        /// <inheritdoc />
        public LogInRefreshRequest()
        {
            this.Action = "login/refresh";
            this.Controller = "auth";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.LoginRefresh;
        }
    }
}