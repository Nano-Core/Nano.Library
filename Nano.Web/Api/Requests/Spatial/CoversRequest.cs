using Nano.Models.Criterias.Interfaces;

namespace Nano.Web.Api.Requests.Spatial
{
    /// <summary>
    /// Covers Request.
    /// </summary>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteriaSpatial"/>.</typeparam>
    public class CoversRequest<TCriteria> : BaseSpatialRequest<TCriteria>
        where TCriteria : IQueryCriteriaSpatial, new()
    {
        /// <inheritdoc />
        public CoversRequest()
        {
            this.Action = "covers";
        }
    }
}