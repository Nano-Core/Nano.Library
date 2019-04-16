using System.Collections.Generic;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class GetResetPasswordTokenRequest : BaseRequestGet
    {
        /// <summary>
        /// User Id.
        /// </summary>
        public virtual string UserId { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GetResetPasswordTokenRequest()
        {
            this.Action = "password/reset/token";
        }

        /// <inheritdoc />
        public override IList<string> GetRouteParameters()
        {
            var parameters = base.GetRouteParameters();

            parameters
                .Add(this.UserId);

            return parameters;
        }
    }
}