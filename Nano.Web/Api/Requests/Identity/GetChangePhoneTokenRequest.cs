using Nano.Web.Api.Requests.Attributes;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class GetChangePhoneTokenRequest : BaseRequestGet
    {
        /// <summary>
        /// Phone Number.
        /// </summary>
        [Query]
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// New Phone Number.
        /// </summary>
        [Query]
        public virtual string NewPhoneNumber { get; set; }

        /// <inheritdoc />
        public GetChangePhoneTokenRequest()
        {
            this.Action = "phone/change/token";
        }
    }
}