using System;
using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class SetUsernameRequest : SetUsernameRequest<Guid>
    {

    }

    /// <inheritdoc />
    public class SetUsernameRequest<TIdentity> : BaseRequestPost
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// Set Username.
        /// </summary>
        public virtual SetUsername<TIdentity> SetUsername { get; set; }

        /// <inheritdoc />
        public SetUsernameRequest()
        {
            this.Action = "username/set";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.SetUsername;
        }
    }
}