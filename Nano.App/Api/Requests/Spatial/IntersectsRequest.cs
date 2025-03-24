using DynamicExpression.Interfaces;

namespace Nano.App.Api.Requests.Spatial;

/// <summary>
/// Intersects Request.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
public class IntersectsRequest<TCriteria> : BaseSpatialRequest<TCriteria>
    where TCriteria : IQueryCriteria, new()
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public IntersectsRequest()
    {
        this.Action = "intersects";
    }
}