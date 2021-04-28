using System;
using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class AssignRoleRequest<TIdentity> : BaseRequestPost
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// Assign Role.
        /// </summary>
        public virtual AssignRole<TIdentity> AssignRole { get; set; }

        /// <inheritdoc />
        public AssignRoleRequest()
        {
            this.Action = "role/assign";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.AssignRole;
        }
    }
}