using System;
using Nano.Web.Api.Requests.Interfaces;

namespace Nano.Web.Api.Requests
{
    /// <summary>
    /// Delete Request.
    /// </summary>
    public class DeleteRequest : BaseRequest, IRequestQueryString
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
            this.Action = $"delete/{this.Id}";
        }
    }
}