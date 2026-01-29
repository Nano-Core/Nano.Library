using System.ComponentModel.DataAnnotations;
using DynamicExpression.Interfaces;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.ApiClient.Requests.Models;
using Nano.App.Consts;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to update entities matching a query.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/> used for filtering.</typeparam>
[PutAction(ActionRoutes.EDIT_QUERY)]
public class EditQueryRequest<TCriteria> : BaseRequest
    where TCriteria : class, IQueryCriteria, new()
{
    /// <summary>
    /// The query defining which entities to update.
    /// </summary>
    [Required]
    [Body]
    public virtual UpdateQuery<TCriteria> Query { get; set; } = new();
}