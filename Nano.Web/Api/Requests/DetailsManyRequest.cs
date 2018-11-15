using System;
using System.Collections.Generic;
using Nano.Web.Api.Requests.Interfaces;

namespace Nano.Web.Api.Requests
{
    /// <summary>
    /// Details Many Request.
    /// </summary>
    public class DetailsManyRequest : BaseRequest, IRequestJson
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
        public object GetBody()
        {
            return this.Ids;
        }
    }
}