using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class AssignUserClaimRequest : AssignUserClaimRequest<Guid>;

/// <summary>
/// Request to assign a claim to a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_CLAIMS_ASSIGN)]
public class AssignUserClaimRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The user claim assignment information.
    /// </summary>
    [Required]
    [Body]
    public virtual AssignUserClaim AssignUserClaim { get; set; } = new();
}