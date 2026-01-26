using DynamicExpression.Interfaces;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to delete multiple entities matching a query.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/> used for filtering.</typeparam>
public class DeleteQueryRequest<TCriteria> : BaseRequestDelete
    where TCriteria : IQueryCriteria, new()
{
    /// <summary>
    /// The query criteria defining which entities to delete.
    /// </summary>
    public virtual TCriteria Criteria { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="DeleteQueryRequest{TCriteria}"/>.
    /// </summary>
    public DeleteQueryRequest()
    {
        this.Action = "delete/query";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.Criteria;
    }
}