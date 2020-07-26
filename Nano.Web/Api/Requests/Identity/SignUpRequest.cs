using System;
using Nano.Models.Interfaces;
using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class SignUpRequest<TUser> : SignUpRequest<TUser, Guid>
        where TUser : IEntityUser<Guid>
    {

    }

    /// <inheritdoc />
    public class SignUpRequest<TUser, TIdentity> : BaseRequestPost
        where TUser : IEntityUser<TIdentity>
    {
        /// <summary>
        /// Sign Up.
        /// </summary>
        public virtual SignUp<TUser, TIdentity> SignUp { get; set; }

        /// <inheritdoc />
        public SignUpRequest()
        {
            this.Action = "signup";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.SignUp;
        }
    }
}