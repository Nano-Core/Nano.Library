using Nano.Models.Criterias.Interfaces;

namespace Nano.App.Api.Requests.Spatial;

/// <summary>
/// Disjoints Request.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteriaSpatial"/>.</typeparam>
public class DisjointsRequest<TCriteria> : BaseSpatialRequest<TCriteria>
    where TCriteria : IQueryCriteriaSpatial, new()
{
    /// <inheritdoc />
    public DisjointsRequest()
    {
        this.Action = "disjoints";
    }
}