using DynamicExpression.Entities;
using DynamicExpression.Interfaces;

namespace Nano.App.Api.Requests;

/// <summary>
/// Query First Request.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
public class QueryFirstRequest<TCriteria> : BaseRequestPost
    where TCriteria : IQueryCriteria, new()
{
    /// <summary>
    /// Query.
    /// </summary>
    public virtual IQuery<TCriteria> Query { get; set; } = new Query<TCriteria>();

    /// <summary>
    /// Constructor.
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