using DynamicExpression.Interfaces;

namespace Nano.App.Api.Requests;

/// <summary>
/// Delete Many Query Request.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
public class DeleteQueryRequest<TCriteria> : BaseRequestDelete
    where TCriteria : IQueryCriteria, new()
{
    /// <summary>
    /// Query Criteria.
    /// </summary>
    public virtual TCriteria Criteria { get; set; } = new();

    /// <summary>
    /// Constructor.
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