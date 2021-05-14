using System;
using Nano.Web.Api.Requests.Attributes;

namespace Nano.Web.Api.Requests.Identity
{
    /// <inheritdoc />
    public class GetRolesRequest : GetRolesRequest<Guid>
    {

    }

    /// <summary>
    /// Get Roles Request.
    /// </summary>
    public class GetRolesRequest<TIdentity> : BaseRequestGet
    {
        /// <summary>
        /// Id.
        /// </summary>
        [Route(Order = 0)]
        public virtual TIdentity Id { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GetRolesRequest()
        {
            this.Action = "roles";
        }
    }
}