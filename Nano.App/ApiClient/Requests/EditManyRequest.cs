using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to update multiple entities at once.
/// </summary>
[PutAction(ActionRoutes.EDIT_MANY)]
public class EditManyRequest : BaseRequest
{
    /// <summary>
    /// The entities to update.
    /// </summary>
    [Required]
    [Body]
    public virtual IEnumerable<IEntityUpdatable> Entities { get; set; } = [];
}