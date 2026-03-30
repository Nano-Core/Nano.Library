using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to create a single entity.
/// </summary>
[PostAction(ActionRoutes.CREATE)]
public class CreateRequest : BaseRequest
{
    /// <summary>
    /// The entity to create.
    /// </summary>
    [Required]
    [Body]
    public virtual IEntityCreatable Entity { get; set; } = null!;
}