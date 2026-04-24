using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a bulk request to create many entities.
/// </summary>
[PostAction(ActionRoutes.CREATE_MANY_BULK)]
public class CreateManyBulkRequest : CreateManyRequest;