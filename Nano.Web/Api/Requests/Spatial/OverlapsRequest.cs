using Nano.Models.Criterias.Interfaces;

namespace Nano.Web.Api.Requests.Spatial
{
    /// <summary>
    /// Overlaps Request.
    /// </summary>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteriaSpatial"/>.</typeparam>
    public class OverlapsRequest<TCriteria> : BaseSpatialRequest<TCriteria>
        where TCriteria : IQueryCriteriaSpatial, new()
    {
        /// <inheritdoc />
        public OverlapsRequest()
        {
            this.Action = "overlaps";
        }
    }
}