using System.Collections.Generic;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class GetChangeEmailTokenRequest : BaseRequestGet
    {
        /// <summary>
        /// Email Address.
        /// </summary>
        public virtual string EmailAddress { get; set; }

        /// <summary>
        /// New Email Address.
        /// </summary>
        public virtual string NewEmailAddress { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GetChangeEmailTokenRequest()
        {
            this.Action = "email/change/token";
        }

        /// <inheritdoc />
        public override IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            var parameters = base.GetQueryStringParameters();

            parameters
                .Add(new KeyValuePair<string, string>("EmailAddress", this.EmailAddress));

            parameters
                .Add(new KeyValuePair<string, string>("NewEmailAddress", this.NewEmailAddress));

            return parameters;
        }
    }
}