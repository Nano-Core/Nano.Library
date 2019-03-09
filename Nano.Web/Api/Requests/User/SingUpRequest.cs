using Nano.Models;
using Nano.Security.Models;

namespace Nano.Web.Api.Requests.User
{
    /// <inheritdoc />
    public class SingUpRequest<TUser> : CreateRequest
        where TUser : DefaultEntityUser
    {
        /// <summary>
        /// Sign Up.
        /// </summary>
        public virtual SignUp<TUser> SignUp { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SingUpRequest()
        {
            this.Controller = "signup";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.SignUp;
        }
    }
}