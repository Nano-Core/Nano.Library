using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class RemoveRoleClaimRequest : RemoveRoleClaimRequest<Guid>;

/// <summary>
/// Represents a request to remove a role's claim.
/// </summary>
/// <typeparam name="TIdentity">The type of the role identifier.</typeparam>
[DeleteAction(ActionRoutes.IDENTITY_ROLES_CLAIMS_REMOVE)]
public class RemoveRoleClaimRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the role claim removal information.
    /// </summary>
    [Required]
    [Body]
    public virtual RemoveClaim RemoveRoleClaim { get; set; } = new();
}