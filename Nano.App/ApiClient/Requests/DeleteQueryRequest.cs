using DynamicExpression.Interfaces;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to delete multiple entities matching a query.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/> used for filtering.</typeparam>
[DeleteAction(ActionRoutes.DELETE_QUERY)]
public class DeleteQueryRequest<TCriteria> : BaseRequest
    where TCriteria : IQueryCriteria, new()
{
    /// <summary>
    /// The query criteria defining which entities to delete.
    /// </summary>
    [Required]
    [Body]
    public virtual TCriteria Criteria { get; set; } = new();
}