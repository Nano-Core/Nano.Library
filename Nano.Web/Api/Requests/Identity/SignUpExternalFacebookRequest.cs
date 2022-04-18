using System;
using Nano.Models.Interfaces;
using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class SignUpExternalFacebookRequest<TUser, TIdentity> : BaseSignUpExternalRequest<SignUpExternalFacebook<TUser, TIdentity>>
        where TUser : IEntityUser<TIdentity>, new()
        where TIdentity : IEquatable<TIdentity>
    {
        /// <inheritdoc />
        public SignUpExternalFacebookRequest()
        {
            this.Action = "signup/external/facebook";
        }
    }
}