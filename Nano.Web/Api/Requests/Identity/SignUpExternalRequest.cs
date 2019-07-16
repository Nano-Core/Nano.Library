using Nano.Models;
using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class SignUpExternalRequest<TUser> : BaseRequestPost
        where TUser : DefaultEntityUser
    {
        /// <summary>
        /// Sign Up External.
        /// </summary>
        public virtual SignUpExternal<TUser> SignUpExternal { get; set; }

        /// <inheritdoc />
        public SignUpExternalRequest()
        {
            this.Action = "signup/external";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.SignUpExternal;
        }
    }
}