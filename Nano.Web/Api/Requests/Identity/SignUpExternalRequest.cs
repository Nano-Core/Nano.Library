using System.Collections.Generic;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class SignUpExternalRequest : BaseRequestPost
    {
        /// <summary>
        /// Login Provider.
        /// </summary>
        public virtual string LoginProvider { get; set; }

        /// <inheritdoc />
        public SignUpExternalRequest()
        {
            this.Action = "external/signup";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return null;
        }

        /// <inheritdoc />
        public override IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            var parameters = base.GetQueryStringParameters();

            parameters
                .Add(new KeyValuePair<string, string>("loginProvider", this.LoginProvider));

            return parameters;
        }
    }
}