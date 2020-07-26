using System;
using Nano.Models.Interfaces;
using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class SignUpExternalRequest<TUser> : SignUpExternalRequest<TUser, Guid>
        where TUser : IEntityUser<Guid>
    {

    }

    /// <inheritdoc />
    public class SignUpExternalRequest<TUser, TIdentity> : BaseRequestPost
        where TUser : IEntityUser<TIdentity>
    {
        /// <summary>
        /// Sign Up External.
        /// </summary>
        public virtual SignUpExternal<TUser, TIdentity> SignUpExternal { get; set; }

        /// <inheritdoc />
        public SignUpExternalRequest()
        {
            this.Action = "signup/external";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.SignUpExternal;
        }
    }

}