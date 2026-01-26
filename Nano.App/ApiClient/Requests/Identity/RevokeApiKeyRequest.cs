using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Requests.Annotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class RevokeApiKeyRequest : RevokeApiKeyRequest<Guid>;

/// <summary>
/// Represents a request to revoke an API key.
/// </summary>
/// <typeparam name="TIdentity">The type of the API key identifier.</typeparam>
public class RevokeApiKeyRequest<TIdentity> : BaseRequestDelete
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

    /// <summary>
    /// Initializes a new instance of <see cref="RevokeApiKeyRequest{TIdentity}"/> with action set.
    /// </summary>
    public RevokeApiKeyRequest()
    {
        this.Action = "api-keys/revoke";
    }

    /// <summary>
    /// Gets the body of the request. Always null for delete requests.
    /// </summary>
    public override object? GetBody()
    {
        return null;
    }
}