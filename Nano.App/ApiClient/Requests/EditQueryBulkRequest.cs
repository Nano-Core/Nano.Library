using DynamicExpression.Interfaces;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to bulk update entities matching a query.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/> used for filtering.</typeparam>
[PutAction(ActionRoutes.EDIT_QUERY_BULK)]
public class EditQueryBulkRequest<TCriteria> : EditQueryRequest<TCriteria>
    where TCriteria : class, IQueryCriteria, new();