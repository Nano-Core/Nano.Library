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
}