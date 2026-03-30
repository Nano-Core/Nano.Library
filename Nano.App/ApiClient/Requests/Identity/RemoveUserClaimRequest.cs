using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class RemoveUserClaimRequest : RemoveUserClaimRequest<Guid>;

/// <summary>
/// Represents a request to remove a user's claim.
/// </summary>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_CLAIMS_REMOVE)]
public class RemoveUserClaimRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the user claim removal information.
    /// </summary>
    [Required]
    [Body]
    public virtual RemoveUserClaim RemoveUserClaim { get; set; } = new();
}