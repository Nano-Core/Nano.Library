using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Requests.Attributes;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class RevokeApiKeyRequest : RevokeApiKeyRequest<Guid>;

/// <inheritdoc />
public class RevokeApiKeyRequest<TIdentity> : BaseRequestDelete
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Api Key Id.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual TIdentity ApiKeyId { get; set; } = default!;

    /// <summary>
    /// Revoke At.
    /// </summary>
    [Query]
    public virtual DateTimeOffset? RevokeAt { get; set; } = new();

    /// <inheritdoc />
    public RevokeApiKeyRequest()
    {
        this.Action = "api-keys/revoke";
    }

    /// <inheritdoc />
    public override object? GetBody()
    {
        return null;
    }
}