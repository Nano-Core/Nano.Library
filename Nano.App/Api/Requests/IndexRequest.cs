using DynamicExpression.Entities;
using DynamicExpression.Interfaces;

namespace Nano.App.Api.Requests;

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
    /// Include Depth.
    /// </summary>
    public virtual int? IncludeDepth { get; set; }

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