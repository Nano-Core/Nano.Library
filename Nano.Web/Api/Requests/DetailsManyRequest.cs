using System;
using System.Collections.Generic;

namespace Nano.Web.Api.Requests
{
    /// <summary>
    /// Details Many Request.
    /// </summary>
    public class DetailsManyRequest : BaseRequestPost
    {
        /// <summary>
        /// Ids.
        /// </summary>
        public virtual ICollection<Guid> Ids { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DetailsManyRequest()
        {
            this.Action = "details";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.Ids;
        }
    }
}