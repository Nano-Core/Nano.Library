using System.ComponentModel.DataAnnotations;
using DynamicExpression.Entities;
using DynamicExpression.Interfaces;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to query data with specified criteria.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/> used for filtering.</typeparam>
[PostAction(ActionRoutes.QUERY)]
public class QueryRequest<TCriteria> : BaseRequest
    where TCriteria : IQueryCriteria, new()
{
    /// <summary>
    /// The query object containing criteria and options.
    /// </summary>
    [Required]
    [Body]
    public virtual IQuery<TCriteria> Query { get; set; } = new Query<TCriteria>();

    /// <summary>
    /// Optional depth for including related entities.
    /// </summary>
    [Query]
    public virtual int? IncludeDepth { get; set; }
}