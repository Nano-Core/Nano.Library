using System.Collections.Generic;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class GetResetPasswordTokenRequest : BaseRequestGet
    {
        /// <summary>
        /// Email Address.
        /// </summary>
        public virtual string EmailAddress { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GetResetPasswordTokenRequest()
        {
            this.Action = "password/reset/token";
        }

        /// <inheritdoc />
        public override IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            var parameters = base.GetQueryStringParameters();

            parameters
                .Add(new KeyValuePair<string, string>("emailAddress", this.EmailAddress));

            return parameters;
        }
    }
}