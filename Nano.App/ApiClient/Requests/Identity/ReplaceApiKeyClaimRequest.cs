using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ReplaceApiKeyClaimRequest : ReplaceApiKeyClaimRequest<Guid>;

/// <summary>
/// Represents a request to replace a api-key's claim.
/// </summary>
/// <typeparam name="TIdentity">The type of the api-key identifier.</typeparam>
[PutAction(ActionRoutes.IDENTITY_API_KEY_CLAIMS_REPLACE)]
public class ReplaceApiKeyClaimRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the api-key.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity ApiKeyId { get; set; }

    /// <summary>
    /// Contains the api-key claim replacement information.
    /// </summary>
    [Required]
    [Body]
    public virtual required ReplaceClaim ReplaceClaim { get; set; }
}