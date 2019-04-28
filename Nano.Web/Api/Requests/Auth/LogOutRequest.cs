namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public class LogOutRequest : BaseRequestPost
    {
        /// <inheritdoc />
        public LogOutRequest()
        {
            this.Action = "logout";
            this.Controller = "auth";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return null;
        }
    }
}