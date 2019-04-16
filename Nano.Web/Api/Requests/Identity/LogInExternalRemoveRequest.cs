using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class LogInExternalRemoveRequest : BaseRequestPost
    {
        /// <summary>
        /// Login.
        /// </summary>
        public virtual LoginExternal LoginExternal { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogInExternalRemoveRequest()
        {
            this.Action = "external/login/remove";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.LoginExternal;
        }
    }
}