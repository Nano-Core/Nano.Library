using Nano.Web.Api.Requests.Attributes;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class GetChangeEmailTokenRequest : BaseRequestGet
    {
        /// <summary>
        /// Email Address.
        /// </summary>
        [Query]
        public virtual string EmailAddress { get; set; }

        /// <summary>
        /// New Email Address.
        /// </summary>
        [Query]
        public virtual string NewEmailAddress { get; set; }

        /// <inheritdoc />
        public GetChangeEmailTokenRequest()
        {
            this.Action = "email/change/token";
        }
    }
}