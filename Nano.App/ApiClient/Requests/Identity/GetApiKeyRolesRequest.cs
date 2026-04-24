using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetApiKeyRolesRequest : GetApiKeyRolesRequest<Guid>;

/// <summary>
/// Represents a request to retrieve the roles assigned to a specific api-key.
/// </summary>
/// <typeparam name="TIdentity">The type of the api-key identifier.</typeparam>
[GetAction(ActionRoutes.IDENTITY_API_KEY_ROLES)]
public class GetApiKeyRolesRequest<TIdentity> : BaseRequest
{
    /// <summary>
    /// The identifier of the api-key.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity ApiKeyId { get; set; }
}