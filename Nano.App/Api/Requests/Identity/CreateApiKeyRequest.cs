using System;
using System.ComponentModel.DataAnnotations;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class CreateApiKeyRequest : CreateApiKeyRequest<Guid>;

/// <inheritdoc />
public class CreateApiKeyRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Create Api Key.
    /// </summary>
    [Required]
    public virtual CreateApiKey<TIdentity> CreateApiKey { get; set; } = new();

    /// <inheritdoc />
    public CreateApiKeyRequest()
    {
        this.Action = "api-keys/create";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.CreateApiKey;
    }
}