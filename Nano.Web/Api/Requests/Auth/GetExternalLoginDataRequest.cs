using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public class GetExternalLoginDataRequest<TProvider> : BaseRequestPost
        where TProvider : BaseLogInExternalProvider, new()
    {
        /// <summary>
        /// Provider.
        /// </summary>
        public virtual TProvider Provider { get; set; } = new();

        /// <inheritdoc />
        public GetExternalLoginDataRequest()
        {
            this.Action = "external/data";
            this.Controller = "auth";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.Provider;
        }
    }
}