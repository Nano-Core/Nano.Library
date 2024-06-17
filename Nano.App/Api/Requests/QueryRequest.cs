using DynamicExpression.Entities;
using DynamicExpression.Interfaces;

namespace Nano.App.Api.Requests;

/// <summary>
/// Query Request.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
public class QueryRequest<TCriteria> : BaseRequestPost
    where TCriteria : IQueryCriteria, new()
{
    /// <summary>
    /// Query.
    /// </summary>
    public virtual IQuery<TCriteria> Query { get; set; } = new Query<TCriteria>();

    /// <summary>
    /// Include Depth.
    /// </summary>
    public virtual int? IncludeDepth { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public QueryRequest()
    {
        this.Action = "query";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.Query;
    }
}