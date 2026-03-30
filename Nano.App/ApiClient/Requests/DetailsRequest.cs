using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests;

/// <inheritdoc />
public class DetailsRequest : DetailsRequest<Guid>;

/// <summary>
/// Represents a request to get details of a single entity by ID.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identifier.</typeparam>
[GetAction(ActionRoutes.DETAILS)]
public class DetailsRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The ID of the entity.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual TIdentity Id { get; set; } = default!;

    /// <summary>
    /// Optional depth for including related entities.
    /// </summary>
    [Query]
    public virtual int? IncludeDepth { get; set; }
}