using DynamicExpression.Interfaces;
using Nano.App.ApiClient.Requests.Models;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to update entities matching a query.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/> used for filtering.</typeparam>
public class EditQueryRequest<TCriteria> : BaseRequestPut
    where TCriteria : class, IQueryCriteria, new()
{
    /// <summary>
    /// The query defining which entities to update.
    /// </summary>
    public virtual UpdateQuery<TCriteria> Query { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="EditQueryRequest{TCriteria}"/>.
    /// </summary>
    public EditQueryRequest()
    {
        this.Action = "edit/query";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.Query;
    }
}