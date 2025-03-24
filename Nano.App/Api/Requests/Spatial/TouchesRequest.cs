using DynamicExpression.Interfaces;

namespace Nano.App.Api.Requests.Spatial;

/// <summary>
/// Touches Request.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
public class TouchesRequest<TCriteria> : BaseSpatialRequest<TCriteria>
    where TCriteria : IQueryCriteria, new()
{
    /// <inheritdoc />
    public TouchesRequest()
    {
        this.Action = "touches";
    }
}