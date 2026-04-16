using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class AssignApiKeyClaimRequest : AssignApiKeyClaimRequest<Guid>;

/// <summary>
/// Request to assign a claim to a api-key.
/// </summary>
/// <typeparam name="TIdentity">Type of the api-key identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_API_KEY_CLAIMS_ASSIGN)]
public class AssignApiKeyClaimRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the api-key.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity ApiKeyId { get; set; }

    /// <summary>
    /// The api-key claim assignment information.
    /// </summary>
    [Required]
    [Body]
    public virtual required AssignClaim AssignClaim { get; set; }
}