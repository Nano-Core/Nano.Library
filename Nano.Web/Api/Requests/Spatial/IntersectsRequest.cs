using Nano.Models.Criterias.Interfaces;

namespace Nano.Web.Api.Requests.Spatial;

/// <summary>
/// Intersects Request.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteriaSpatial"/>.</typeparam>
public class IntersectsRequest<TCriteria> : BaseSpatialRequest<TCriteria>
    where TCriteria : IQueryCriteriaSpatial, new()
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public IntersectsRequest()
    {
        this.Action = "intersects";
    }
}