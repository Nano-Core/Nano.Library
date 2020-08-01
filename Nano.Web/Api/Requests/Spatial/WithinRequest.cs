using System.Collections.Generic;
using System.Globalization;
using Nano.Models.Criterias.Interfaces;

namespace Nano.Web.Api.Requests.Spatial
{
    /// <summary>
    /// Within Request.
    /// </summary>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteriaSpatial"/>.</typeparam>
    public class WithinRequest<TCriteria> : BaseSpatialRequest<TCriteria>
        where TCriteria : IQueryCriteriaSpatial, new()
    {
        /// <summary>
        /// Distance.
        /// </summary>
        public virtual int Distance { get; set; }

        /// <inheritdoc />
        public WithinRequest()
        {
            this.Action = "within";
        }

        /// <inheritdoc />
        public override IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            var parameters = base.GetQueryStringParameters();

            parameters
                .Add(new KeyValuePair<string, string>("distance", this.Distance.ToString(CultureInfo.InvariantCulture)));

            return parameters;
        }
    }
}