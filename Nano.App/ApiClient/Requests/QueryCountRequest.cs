using System.ComponentModel.DataAnnotations;
using DynamicExpression.Interfaces;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to count entities based on specified criteria.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/> used for filtering.</typeparam>
[PostAction(ActionRoutes.QUERY_COUNT)]
public class QueryCountRequest<TCriteria> : BaseRequest
    where TCriteria : IQueryCriteria, new()
{
    /// <summary>
    /// The criteria used to filter entities.
    /// </summary>
    [Required]
    [Body]
    public virtual TCriteria Criteria { get; set; } = new();
}