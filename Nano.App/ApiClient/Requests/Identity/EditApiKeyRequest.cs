using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class EditApiKeyRequest : EditApiKeyRequest<Guid>;

/// <summary>
/// Request to edit an API key for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[PutAction(ActionRoutes.IDENTITY_API_KEYS_EDIT)]
public class EditApiKeyRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The ID of the API key to revoke.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity ApiKeyId { get; set; }

    /// <summary>
    /// The API key edit information.
    /// </summary>
    [Required]
    [Body]
    public virtual required EditApiKey EditApiKey { get; set; }
}