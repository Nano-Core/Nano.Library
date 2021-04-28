using System;
using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class SetPasswordRequest : BaseRequestPost
    {
        /// <summary>
        /// Set Password.
        /// </summary>
        public virtual SetPassword<Guid> SetPassword { get; set; }

        /// <inheritdoc />
        public SetPasswordRequest()
        {
            this.Action = "password/set";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.SetPassword;
        }
    }
}