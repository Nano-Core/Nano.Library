using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to edit an entity and retrieve it.
/// </summary>
[PutAction(ActionRoutes.EDIT_GET)]
public class EditAndGetRequest : BaseRequest
{
    /// <summary>
    /// The entity to update.
    /// </summary>
    [Required]
    [Body]
    public virtual IEntityUpdatable Entity { get; set; } = null!;
}