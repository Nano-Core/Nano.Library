using DynamicExpression.Interfaces;
using Nano.Models.Criterias;
using Nano.Models.Interfaces;

namespace Nano.Web.Api.Requests;

/// <summary>
/// Update Many Query Request.
/// </summary>
public class EditQueryRequest : BaseRequestPut
{
    /// <summary>
    /// Entity.
    /// </summary>
    public virtual IEntityUpdatable Entity { get; set; }

    /// <summary>
    /// Query Criteria.
    /// </summary>
    public virtual IQueryCriteria QueryCriteria { get; set; } = new DefaultQueryCriteria();

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
        return (this.Entity, this.QueryCriteria);
    }
}