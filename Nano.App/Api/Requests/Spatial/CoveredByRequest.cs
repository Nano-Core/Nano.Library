using DynamicExpression.Interfaces;

namespace Nano.App.Api.Requests.Spatial;

/// <summary>
/// Covered-By Request.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
public class CoveredByRequest<TCriteria> : BaseSpatialRequest<TCriteria>
    where TCriteria : IQueryCriteria, new()
{
    /// <inheritdoc />
    public CoveredByRequest()
    {
        this.Action = "covered-by";
    }
}