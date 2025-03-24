using DynamicExpression.Interfaces;

namespace Nano.App.Api.Requests.Spatial;

/// <summary>
/// Overlaps Request.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
public class OverlapsRequest<TCriteria> : BaseSpatialRequest<TCriteria>
    where TCriteria : IQueryCriteria, new()
{
    /// <inheritdoc />
    public OverlapsRequest()
    {
        this.Action = "overlaps";
    }
}