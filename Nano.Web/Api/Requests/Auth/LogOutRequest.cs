using Nano.Models.Auth;

namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public class LogOutRequest : BaseRequestGet
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public LogOutRequest()
        {
            this.Action = "logout";
            this.Controller = "auth";
        }
    }

    /// <inheritdoc />
    public class LogInRequest : BaseRequestPost
    {
        /// <summary>
        /// Login.
        /// </summary>
        public virtual Login Login { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LogInRequest()
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