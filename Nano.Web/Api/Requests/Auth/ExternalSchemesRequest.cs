namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public class ExternalSchemesRequest : BaseRequestGet
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ExternalSchemesRequest()
        {
            this.Action = "external/schemes";
            this.Controller = "auth";
        }
    }
}