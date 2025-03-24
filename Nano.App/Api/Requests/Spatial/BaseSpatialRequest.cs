using DynamicExpression.Interfaces;
using Nano.App.Api.Requests.Attributes;
using Nano.Models.Criterias;

namespace Nano.App.Api.Requests.Spatial;

/// <summary>
/// Base Spatial Request (abstract).
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
public abstract class BaseSpatialRequest<TCriteria> : BaseRequestPost
    where TCriteria : IQueryCriteria, new()
{
    /// <summary>
    /// Query.
    /// </summary>
    public virtual SpatialQuery<TCriteria> Query { get; set; } = new();

    /// <summary>
    /// Include Depth.
    /// </summary>
    [Query]
    public virtual int? IncludeDepth { get; set; }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.Query;
    }
}