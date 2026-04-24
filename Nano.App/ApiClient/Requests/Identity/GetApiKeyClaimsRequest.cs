using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetApiKeyClaimsRequest : GetApiKeyClaimsRequest<Guid>;

/// <summary>
/// Request to retrieve claims for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the api-key identifier.</typeparam>
[GetAction(ActionRoutes.IDENTITY_API_KEY_CLAIMS)]
public class GetApiKeyClaimsRequest<TIdentity> : BaseRequest
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity ApiKeyId { get; set; }
}