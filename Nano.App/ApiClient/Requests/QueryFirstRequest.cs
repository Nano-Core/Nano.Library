using DynamicExpression.Entities;
using DynamicExpression.Interfaces;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to query the first matching entity.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/> used for filtering.</typeparam>
public class QueryFirstRequest<TCriteria> : BaseRequestPost
    where TCriteria : IQueryCriteria, new()
{
    /// <summary>
    /// The query object containing criteria and options.
    /// </summary>
    public virtual IQuery<TCriteria> Query { get; set; } = new Query<TCriteria>();

    /// <summary>
    /// Optional depth for including related entities.
    /// </summary>
    public virtual int? IncludeDepth { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="QueryFirstRequest{TCriteria}"/>.
    /// </summary>
    public QueryFirstRequest()
    {
        this.Action = "query/first";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.Query;
    }
}