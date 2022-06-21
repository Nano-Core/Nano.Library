using DynamicExpression.Entities;
using DynamicExpression.Interfaces;

namespace Nano.Web.Api.Requests;

/// <summary>
/// Index Request.
/// </summary>
public class IndexRequest : BaseRequestPost
{
    /// <summary>
    /// Query.
    /// </summary>
    public virtual IQuery Query { get; set; } = new Query();

    /// <summary>
    /// Constructor.
    /// </summary>
    public IndexRequest()
    {
        this.Action = "index";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.Query;
    }
}