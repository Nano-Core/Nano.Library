using System;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;

namespace Nano.App.ApiClient.Requests;

/// <inheritdoc />
public class DeleteManyBulkRequest : DeleteManyBulkRequest<Guid>;

/// <summary>
/// Represents a bulk request to delete many entities by their IDs.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identifiers.</typeparam>
[DeleteAction(ActionRoutes.DELETE_MANY_BULK)]
public class DeleteManyBulkRequest<TIdentity> : DeleteManyRequest<TIdentity>;