using System;
using System.Collections.Generic;

namespace Nano.Web.Api.Requests
{
    /// <inheritdoc />
    public class DetailsRequest : DetailsRequest<Guid>
    {

    }

    /// <summary>
    /// Details Request.
    /// </summary>
    public class DetailsRequest<TIdentity> : BaseRequestGet
    {
        /// <summary>
        /// Id.
        /// </summary>
        public virtual TIdentity Id { get; set; }

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