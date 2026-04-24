using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class CreateApiKeyRequest : CreateApiKeyRequest<Guid>;

/// <summary>
/// Request to create an API key for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_API_KEYS_CREATE)]
public class CreateApiKeyRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity Id { get; set; }

    /// <summary>
    /// The API key information to create.
    /// </summary>
    [Required]
    [Body]
    public virtual required CreateApiKey CreateApiKey { get; set; }
}