using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class RemoveApiKeyRoleRequest : RemoveApiKeyRoleRequest<Guid>;

/// <summary>
/// Represents a request to remove a role from a api-key.
/// </summary>
/// <typeparam name="TIdentity">The type of the api-key identifier.</typeparam>
[DeleteAction(ActionRoutes.IDENTITY_API_KEY_ROLES_REMOVE)]
public class RemoveApiKeyRoleRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the api-key.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity ApiKeyId { get; set; }

    /// <summary>
    /// Contains the api-key role removal information.
    /// </summary>
    [Required]
    [Body]
    public virtual required RemoveRole RemoveRole { get; set; }
}