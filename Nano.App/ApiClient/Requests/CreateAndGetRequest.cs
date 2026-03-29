using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to create an entity and retrieve it.
/// </summary>
[PostAction(ActionRoutes.CREATE_AND_GET)]
public class CreateAndGetRequest : BaseRequest
{
    /// <summary>
    /// The entity to create.
    /// </summary>
    [Required]
    [Body]
    public virtual IEntityCreatable Entity { get; set; } = null!;
}