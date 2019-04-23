using Nano.Models;
using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class SingUpExternalRequest<TUser> : BaseRequestPost
        where TUser : DefaultEntityUser
    {
        /// <summary>
        /// Sign Up External.
        /// </summary>
        public virtual SignUpExternal<TUser> SignUpExternal { get; set; }

        /// <inheritdoc />
        public SingUpExternalRequest()
        {
            this.Action = "external/signup";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.SignUpExternal;
        }
    }
}