using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class RemoveExternalLogInRequest : BaseRequestPost
    {
        /// <summary>
        /// Login External.
        /// </summary>
        public virtual LoginExternal LoginExternal { get; set; }

        /// <inheritdoc />
        public RemoveExternalLogInRequest()
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