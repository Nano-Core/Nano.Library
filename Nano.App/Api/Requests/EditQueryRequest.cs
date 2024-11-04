using DynamicExpression.Interfaces;
using Nano.App.Api.Requests.Models;

namespace Nano.App.Api.Requests;

/// <summary>
/// Edit Query Request.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
public class EditQueryRequest<TCriteria> : BaseRequestPut
    where TCriteria : IQueryCriteria, new()
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