using System;
using System.Collections.Generic;

namespace Nano.Web.Api.Requests
{
    /// <summary>
    /// Delete Request.
    /// </summary>
    public class DeleteRequest : BaseRequestDelete
    {
        /// <summary>
        /// Id.
        /// </summary>
        public virtual Guid Id { get; set; }

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