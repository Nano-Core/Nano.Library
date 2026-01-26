using System;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class CreateApiKeyRequest : CreateApiKeyRequest<Guid>;

/// <summary>
/// Request to create an API key for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class CreateApiKeyRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The API key information to create.
    /// </summary>
    [Required]
    public virtual CreateApiKey<TIdentity> CreateApiKey { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="CreateApiKeyRequest{TIdentity}"/>.
    /// Sets the action to "api-keys/create".
    /// </summary>
    public CreateApiKeyRequest()
    {
        this.Action = "api-keys/create";
    }

    /// <summary>
    /// Gets the request body containing the API key to create.
    /// </summary>
    public override object GetBody()
    {
        return this.CreateApiKey;
    }
}