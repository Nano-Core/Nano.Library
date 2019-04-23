using System.Collections.Generic;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class GetConfirmEmailTokenRequest : BaseRequestGet
    {
        /// <summary>
        /// Email Address.
        /// </summary>
        public virtual string EmailAddress { get; set; }

        /// <inheritdoc />
        public GetConfirmEmailTokenRequest()
        {
            this.Action = "email/confirm/token";
        }

        /// <inheritdoc />
        public override IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            var parameters = base.GetQueryStringParameters();

            parameters
                .Add(new KeyValuePair<string, string>("EmailAddress", this.EmailAddress));

            return parameters;
        }
    }
}