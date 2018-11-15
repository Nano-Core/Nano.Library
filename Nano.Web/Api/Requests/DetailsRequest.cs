using System;
using Nano.Web.Api.Requests.Interfaces;

namespace Nano.Web.Api.Requests
{
    /// <summary>
    /// Details Request.
    /// </summary>
    public class DetailsRequest : BaseRequest, IRequestQueryString
    {
        /// <summary>
        /// Id.
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DetailsRequest()
        {
            this.Action = "details";
        }
    }
}