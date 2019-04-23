namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public class LogOutRequest : BaseRequestGet
    {
        /// <inheritdoc />
        public LogOutRequest()
        {
            this.Action = "logout";
            this.Controller = "auth";
        }
    }
}