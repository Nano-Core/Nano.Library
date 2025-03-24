using DynamicExpression.Interfaces;

namespace Nano.App.Api.Requests.Spatial;

/// <summary>
/// Covers Request.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
public class CoversRequest<TCriteria> : BaseSpatialRequest<TCriteria>
    where TCriteria : IQueryCriteria, new()
{
    /// <inheritdoc />
    public CoversRequest()
    {
        this.Action = "covers";
    }
}