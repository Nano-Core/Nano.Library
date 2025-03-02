using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.Api.Requests.Attributes;

namespace Nano.App.Api.Requests.Identity;

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
    [Route]
    public virtual TIdentity ApiKeyId { get; set; }

    /// <summary>
    /// Revoke At.
    /// </summary>
    public virtual DateTimeOffset? RevokeAt { get; set; } = new();

    /// <inheritdoc />
    public RevokeApiKeyRequest()
    {
        this.Action = "api-keys/revoke";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return null;
    }
}