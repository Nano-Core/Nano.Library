using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class ConfirmPhoneRequest : BaseRequestPost
    {
        /// <summary>
        /// Confirm Phone.
        /// </summary>
        public virtual ConfirmEmail ConfirmPhone { get; set; } = new();

        /// <inheritdoc />
        public ConfirmPhoneRequest()
        {
            this.Action = "phone/confirm";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.ConfirmPhone;
        }
    }
}