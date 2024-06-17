using DynamicExpression.Entities;
using DynamicExpression.Interfaces;
using Nano.App.Api.Requests.Attributes;
using Nano.Models.Criterias.Interfaces;

namespace Nano.App.Api.Requests.Spatial;

/// <summary>
/// Base Spatial Request (abstract).
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteriaSpatial"/>.</typeparam>
public abstract class BaseSpatialRequest<TCriteria> : BaseRequestPost
    where TCriteria : IQueryCriteriaSpatial, new()
{
    /// <summary>
    /// Query.
    /// </summary>
    public virtual IQuery<TCriteria> Query { get; set; } = new Query<TCriteria>();

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