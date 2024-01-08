using Nano.Models.Criterias.Interfaces;

namespace Nano.App.Api.Requests.Spatial;

/// <summary>
/// Covered-By Request.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteriaSpatial"/>.</typeparam>
public class CoveredByRequest<TCriteria> : BaseSpatialRequest<TCriteria>
    where TCriteria : IQueryCriteriaSpatial, new()
{
    /// <inheritdoc />
    public CoveredByRequest()
    {
        this.Action = "covered-by";
    }
}