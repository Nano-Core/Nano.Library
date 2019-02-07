using System;
using System.Collections.Generic;

namespace Nano.Web.Api.Requests
{
    /// <summary>
    /// Delete Many Request.
    /// </summary>
    public class DeleteManyRequest : BaseRequestPost
    {
        /// <summary>
        /// Ids.
        /// </summary>
        public virtual IEnumerable<Guid> Ids { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DeleteManyRequest()
        {
            this.Action = "delete/many";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.Ids;
        }
    }
}