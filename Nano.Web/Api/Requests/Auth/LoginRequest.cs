using Nano.Models;

namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public class LoginRequest : BaseRequestJson
    {
        /// <summary>
        /// Login.
        /// </summary>
        public virtual Login Login { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LoginRequest()
        {
            this.Action = "login";
            this.Controller = "auth";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.Login;
        }
    }
}