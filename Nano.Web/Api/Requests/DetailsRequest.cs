using System;
using System.Collections.Generic;
using Nano.Web.Api.Requests.Interfaces;

namespace Nano.Web.Api.Requests
{
    /// <summary>
    /// Details Request.
    /// </summary>
    public class DetailsRequest : BaseRequest, IRequestQuerystring
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