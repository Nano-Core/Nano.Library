using DynamicExpression.Entities;
using DynamicExpression.Interfaces;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to index entities using a query.
/// </summary>
public class IndexRequest : BaseRequestPost
{
    /// <summary>
    /// The query defining entities to index.
    /// </summary>
    public virtual IQuery Query { get; set; } = new Query();

    /// <summary>
    /// Optional depth for including related entities.
    /// </summary>
    public virtual int? IncludeDepth { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="IndexRequest"/>.
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