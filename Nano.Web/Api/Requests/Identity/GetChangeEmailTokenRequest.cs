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

        /// <inheritdoc />
        public GetChangeEmailTokenRequest()
        {
            this.Action = "email/change/token";
        }

        /// <inheritdoc />
        public override IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            var parameters = base.GetQueryStringParameters();

            parameters
                .Add(new KeyValuePair<string, string>("emailAddress", this.EmailAddress));
            
            parameters
                .Add(new KeyValuePair<string, string>("newEmailAddress", this.NewEmailAddress));

            return parameters;
        }
    }
}