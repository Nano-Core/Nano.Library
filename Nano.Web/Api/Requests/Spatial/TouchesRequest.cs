using Nano.Models.Criterias.Interfaces;

namespace Nano.Web.Api.Requests.Spatial
{
    /// <summary>
    /// Touches Request.
    /// </summary>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteriaSpatial"/>.</typeparam>
    public class TouchesRequest<TCriteria> : BaseSpatialRequest<TCriteria>
        where TCriteria : IQueryCriteriaSpatial, new()
    {
        /// <inheritdoc />
        public TouchesRequest()
        {
            this.Action = "touches";
        }
    }
}