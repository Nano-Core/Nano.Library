using DynamicExpression.Interfaces;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to bulk delete multiple entities matching a query.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/> used for filtering.</typeparam>
[DeleteAction(ActionRoutes.DELETE_QUERY_BULK)]
public class DeleteQueryBulkRequest<TCriteria> : DeleteQueryRequest<TCriteria>
    where TCriteria : IQueryCriteria, new();