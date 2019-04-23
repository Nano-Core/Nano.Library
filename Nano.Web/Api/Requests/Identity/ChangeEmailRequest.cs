using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class ChangeEmailRequest : BaseRequestPost
    {
        /// <summary>
        /// Change Email.
        /// </summary>
        public virtual ChangeEmail ChangeEmail { get; set; }

        /// <inheritdoc />
        public ChangeEmailRequest()
        {
            this.Action = "email/change";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.ChangeEmail;
        }
    }
}