using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to create or edit a single entity.
/// </summary>
[PostAction(ActionRoutes.CREATE_OR_EDIT)]
public class CreateOrEditRequest : BaseRequest
{
    /// <summary>
    /// The entity to create.
    /// </summary>
    [Required]
    [Body]
    public virtual required IEntityCreatableAndUpdatable Entity { get; set; }
}