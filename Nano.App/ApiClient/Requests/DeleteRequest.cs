using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests;

/// <inheritdoc />
public class DeleteRequest : DeleteRequest<Guid>;

/// <summary>
/// Represents a request to delete an entity by ID.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identifier.</typeparam>
[DeleteAction(ActionRoutes.DELETE)]
public class DeleteRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The ID of the entity to delete.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual TIdentity Id { get; set; } = default!;
}