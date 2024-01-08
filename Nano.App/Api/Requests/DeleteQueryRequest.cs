using DynamicExpression.Interfaces;
using Nano.Models.Criterias;

namespace Nano.App.Api.Requests;

/// <summary>
/// Delete Many Query Request.
/// </summary>
public class DeleteQueryRequest : BaseRequestDelete
{
    /// <summary>
    /// Query Criteria.
    /// </summary>
    public virtual IQueryCriteria QueryCriteria { get; set; } = new DefaultQueryCriteria();

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
        return this.QueryCriteria;
    }
}