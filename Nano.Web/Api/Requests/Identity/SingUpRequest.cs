using Nano.Models;
using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class SingUpRequest<TUser> : BaseRequestPost
        where TUser : DefaultEntityUser
    {
        /// <summary>
        /// Sign Up.
        /// </summary>
        public virtual SignUp<TUser> SignUp { get; set; }

        /// <inheritdoc />
        public SingUpRequest()
        {
            this.Action = "signup";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.SignUp;
        }
    }
}