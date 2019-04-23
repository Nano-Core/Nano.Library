using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class SetUsernameRequest : BaseRequestPost
    {
        /// <summary>
        /// Set Username.
        /// </summary>
        public virtual SetUsername SetUsername { get; set; }

        /// <inheritdoc />
        public SetUsernameRequest()
        {
            this.Action = "username/set";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.SetUsername;
        }
    }
}