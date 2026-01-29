using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;

namespace Nano.App.ApiClient.Requests;

/// <inheritdoc />
public class DeleteManyRequest : DeleteManyRequest<Guid>;

/// <summary>
/// Represents a request to delete multiple entities by their IDs.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identifiers.</typeparam>
[DeleteAction(ActionRoutes.DELETE_MANY)]
public class DeleteManyRequest<TIdentity> : BaseRequest
{
    /// <summary>
    /// The IDs of the entities to delete.
    /// </summary>
    [Required]
    [Body]
    public virtual IEnumerable<TIdentity> Ids { get; set; } = [];
}