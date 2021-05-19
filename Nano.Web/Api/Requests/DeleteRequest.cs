using System;
using Nano.Web.Api.Requests.Attributes;

namespace Nano.Web.Api.Requests
{
    /// <inheritdoc />
    public class DeleteRequest : DeleteRequest<Guid>
    {

    }

    /// <summary>
    /// Delete Request.
    /// </summary>
    public class DeleteRequest<TIdentity> : BaseRequestDelete
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// Id.
        /// </summary>
        [Route]
        public virtual TIdentity Id { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DeleteRequest()
        {
            this.Action = "delete";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return null;
        }
    }
}