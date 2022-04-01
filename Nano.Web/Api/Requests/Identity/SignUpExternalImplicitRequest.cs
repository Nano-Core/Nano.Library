using System;
using Nano.Models.Interfaces;
using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class SignUpExternalImplicitRequest<TUser, TIdentity> : BaseSignUpExternalRequest<SignUpExternalImplicit<TUser, TIdentity>>
        where TUser : IEntityUser<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        /// <inheritdoc />
        public SignUpExternalImplicitRequest()
        {
            this.Action = "signup/external/implicit";
        }
    }
}