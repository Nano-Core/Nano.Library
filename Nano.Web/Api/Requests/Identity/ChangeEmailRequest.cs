using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class ChangeEmailRequest : BaseRequestPost
    {
        /// <summary>
        /// Change Password.
        /// </summary>
        public virtual ChangeEmail ChangeEmail { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
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