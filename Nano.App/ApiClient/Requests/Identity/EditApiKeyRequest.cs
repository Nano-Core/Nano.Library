using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Requests.Attributes;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class EditApiKeyRequest : EditApiKeyRequest<Guid>;

/// <inheritdoc />
public class EditApiKeyRequest<TIdentity> : BaseRequestPut
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Id.
    /// </summary>
    [Required]
    [Route]
    public virtual TIdentity Id { get; set; }

    /// <summary>
    /// Create Api Key.
    /// </summary>
    [Required]
    public virtual EditApiKey EditApiKey { get; set; } = new();

    /// <inheritdoc />
    public EditApiKeyRequest()
    {
        this.Action = "api-keys/edit";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.EditApiKey;
    }
}