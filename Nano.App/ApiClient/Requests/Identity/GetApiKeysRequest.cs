using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetApiKeysRequest : GetApiKeysRequest<Guid>;

/// <summary>
/// Request to retrieve API keys for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[GetAction(ActionRoutes.IDENTITY_API_KEYS)]
public class GetApiKeysRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity Id { get; set; }
}