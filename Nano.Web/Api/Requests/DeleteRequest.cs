using System;
using System.Collections.Generic;

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
    {
        /// <summary>
        /// Id.
        /// </summary>
        public virtual TIdentity Id { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DeleteRequest()
        {
            this.Action = "delete";
        }

        /// <inheritdoc />
        public override IList<string> GetRouteParameters()
        {
            var parameters = base.GetRouteParameters();

            parameters
                .Add(this.Id.ToString());

            return parameters;
        }
    }
}