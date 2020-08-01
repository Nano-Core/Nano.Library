using Nano.Models.Criterias.Interfaces;

namespace Nano.Web.Api.Requests.Spatial
{
    /// <summary>
    /// Crosses Request.
    /// </summary>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteriaSpatial"/>.</typeparam>
    public class CrossesRequest<TCriteria> : BaseSpatialRequest<TCriteria>
        where TCriteria : IQueryCriteriaSpatial, new()
    {
        /// <inheritdoc />
        public CrossesRequest()
        {
            this.Action = "crosses";
        }
    }
}