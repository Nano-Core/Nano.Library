using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class ChangePasswordRequest : BaseRequestPost
    {
        /// <summary>
        /// Change Password.
        /// </summary>
        public virtual ChangePassword ChangePassword { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ChangePasswordRequest()
        {
            this.Action = "password/change";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.ChangePassword;
        }
    }
}