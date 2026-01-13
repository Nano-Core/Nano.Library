using DynamicExpression.Interfaces;
using Nano.App.ApiClient.Requests.Models;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Edit Query Request.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
public class EditQueryRequest<TCriteria> : BaseRequestPut
    where TCriteria : class, IQueryCriteria, new()
{
    /// <summary>
    /// Query.
    /// </summary>
    public virtual UpdateQuery<TCriteria> Query { get; set; } = new();

    /// <summary>
    /// Constructor.
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