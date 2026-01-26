using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Requests.Annotations;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class EditApiKeyRequest : EditApiKeyRequest<Guid>;

/// <summary>
/// Request to edit an API key for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class EditApiKeyRequest<TIdentity> : BaseRequestPut
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the API key.
    /// </summary>
    [Required]
    [Route]
    public virtual TIdentity Id { get; set; } = default!;

    /// <summary>
    /// The API key edit information.
    /// </summary>
    [Required]
    public virtual EditApiKey EditApiKey { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="EditApiKeyRequest{TIdentity}"/>.
    /// Sets the action to "api-keys/edit".
    /// </summary>
    public EditApiKeyRequest()
    {
        this.Action = "api-keys/edit";
    }

    /// <summary>
    /// Gets the request body containing the API key edit information.
    /// </summary>
    public override object GetBody()
    {
        return this.EditApiKey;
    }
}