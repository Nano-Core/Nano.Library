using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class DetailsDeactivatedRequest : DetailsDeactivatedRequest<Guid>;

/// <summary>
/// Represents a request to get details of a single entity by ID.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identifier.</typeparam>
[GetAction(ActionRoutes.IDENTITY_DETAILS_DEACTIVATED)]
public class DetailsDeactivatedRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The ID of the entity.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity Id { get; set; }
}