using System;
using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class ChangeEmailRequest : ChangeEmailRequest<Guid>
    {

    }

    /// <inheritdoc />
    public class ChangeEmailRequest<TIdentity> : BaseRequestPost
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// Change Email.
        /// </summary>
        public virtual ChangeEmail<TIdentity> ChangeEmail { get; set; }

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