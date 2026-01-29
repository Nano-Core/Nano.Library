using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to edit a single entity.
/// </summary>
[PutAction(ActionRoutes.EDIT)]
public class EditRequest : BaseRequest
{
    /// <summary>
    /// The entity to update.
    /// </summary>
    [Required]
    [Body]
    public virtual IEntityUpdatable Entity { get; set; } = null!;
}