using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to create multiple entities at once.
/// </summary>
[PostAction(ActionRoutes.CREATE_MANY)]
public class CreateManyRequest : BaseRequest
{
    /// <summary>
    /// The entities to create.
    /// </summary>
    [Required]
    [Body]
    public virtual IEnumerable<IEntityCreatable> Entities { get; set; } = [];
}