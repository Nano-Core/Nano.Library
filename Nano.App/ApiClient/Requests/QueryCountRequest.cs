using DynamicExpression.Interfaces;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to count entities based on specified criteria.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/> used for filtering.</typeparam>
public class QueryCountRequest<TCriteria> : BaseRequestPost
    where TCriteria : IQueryCriteria, new()
{
    /// <summary>
    /// The criteria used to filter entities.
    /// </summary>
    public virtual TCriteria Criteria { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="QueryCountRequest{TCriteria}"/>.
    /// </summary>
    public QueryCountRequest()
    {
        this.Action = "query/count";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.Criteria;
    }
}