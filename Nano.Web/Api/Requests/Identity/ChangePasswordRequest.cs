using System;
using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class ChangePasswordRequest<TIdentity> : BaseRequestPost
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// Change Password.
        /// </summary>
        public virtual ChangePassword<TIdentity> ChangePassword { get; set; }

        /// <inheritdoc />
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