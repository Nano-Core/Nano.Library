using System;
using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class RemoveRoleRequest<TIdentity> : BaseRequestPost
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// Assign Role.
        /// </summary>
        public virtual RemoveRole<TIdentity> RemoveRole { get; set; }

        /// <inheritdoc />
        public RemoveRoleRequest()
        {
            this.Action = "role/remove";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.RemoveRole;
        }
    }
}