using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a bulk request to update many entities.
/// </summary>
[PutAction(ActionRoutes.EDIT_MANY_BULK)]
public class EditManyBulkRequest : EditManyRequest;