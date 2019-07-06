using System.Collections.Generic;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class SignUpExternalCallbackRequest : BaseRequestGet
    {
        /// <summary>
        /// Remote Error.
        /// </summary>
        public virtual string RemoteError { get; set; }

        /// <inheritdoc />
        public SignUpExternalCallbackRequest()
        {
            this.Action = "external/signup/callback";
        }

        /// <inheritdoc />
        public override IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            var parameters = base.GetQueryStringParameters();

            parameters
                .Add(new KeyValuePair<string, string>("remoteError", this.RemoteError));

            return parameters;
        }
    }
}