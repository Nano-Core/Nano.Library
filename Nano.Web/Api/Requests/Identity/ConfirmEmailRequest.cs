using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class ConfirmEmailRequest : BaseRequestPost
    {
        /// <summary>
        /// Confirm Email.
        /// </summary>
        public virtual ConfirmEmail ConfirmEmail { get; set; }

        /// <inheritdoc />
        public ConfirmEmailRequest()
        {
            this.Action = "email/confirm";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.ConfirmEmail;
        }
    }
}