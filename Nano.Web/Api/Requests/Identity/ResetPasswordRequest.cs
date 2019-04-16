using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class ResetPasswordRequest : BaseRequestPost
    {
        /// <summary>
        /// Set Username.
        /// </summary>
        public virtual ResetPassword ResetPassword { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ResetPasswordRequest()
        {
            this.Action = "password/reset";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.ResetPassword;
        }
    }
}