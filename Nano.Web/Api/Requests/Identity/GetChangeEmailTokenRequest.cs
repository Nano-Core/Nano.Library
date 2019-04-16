using System.Collections.Generic;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class GetChangeEmailTokenRequest : BaseRequestGet
    {
        /// <summary>
        /// User Id.
        /// </summary>
        public virtual string UserId { get; set; }

        /// <summary>
        /// New Email.
        /// </summary>
        public virtual string NewEmail { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GetChangeEmailTokenRequest()
        {
            this.Action = "email/change/token";
        }

        /// <inheritdoc />
        public override IList<string> GetRouteParameters()
        {
            var parameters = base.GetRouteParameters();

            parameters
                .Add(this.UserId);

            return parameters;
        }

        /// <inheritdoc />
        public override IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            var parameters = base.GetQueryStringParameters();

            parameters
                .Add(new KeyValuePair<string, string>("newEmail", this.NewEmail));

            return parameters;
        }
    }
}