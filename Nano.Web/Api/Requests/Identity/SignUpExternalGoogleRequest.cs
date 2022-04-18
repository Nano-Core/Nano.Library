using System;
using Nano.Models.Interfaces;
using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class SignUpExternalGoogleRequest<TUser, TIdentity> : BaseSignUpExternalRequest<SignUpExternalGoogle<TUser, TIdentity>>
        where TUser : IEntityUser<TIdentity>, new()
        where TIdentity : IEquatable<TIdentity>
    {
        /// <inheritdoc />
        public SignUpExternalGoogleRequest()
        {
            this.Action = "signup/external/google";
        }
    }
}