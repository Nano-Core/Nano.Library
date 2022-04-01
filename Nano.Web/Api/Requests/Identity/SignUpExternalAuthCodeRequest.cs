using System;
using Nano.Models.Interfaces;
using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class SignUpExternalAuthCodeRequest<TUser, TIdentity> : BaseSignUpExternalRequest<SignUpExternalAuthCode<TUser, TIdentity>>
        where TUser : IEntityUser<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        /// <inheritdoc />
        public SignUpExternalAuthCodeRequest()
        {
            this.Action = "signup/external/authcode";
        }
    }
}