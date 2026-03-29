using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class RevokeApiKeyRequest : RevokeApiKeyRequest<Guid>;

/// <summary>
/// Represents a request to revoke an API key.
/// </summary>
/// <typeparam name="TIdentity">The type of the API key identifier.</typeparam>
[DeleteAction(ActionRoutes.IDENTITY_API_KEYS_REVOKE)]
public class RevokeApiKeyRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The ID of the API key to revoke.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual TIdentity ApiKeyId { get; set; } = default!;

    /// <summary>
    /// Optional time at which the API key should be revoked.
    /// </summary>
    [Query]
    public virtual DateTimeOffset? RevokeAt { get; set; } = new();
}