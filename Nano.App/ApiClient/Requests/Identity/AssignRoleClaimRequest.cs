using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class AssignRoleClaimRequest : AssignRoleClaimRequest<Guid>;

/// <summary>
/// Request to assign a claim to a role.
/// </summary>
/// <typeparam name="TIdentity">Type of the role identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_ROLES_CLAIMS_ASSIGN)]
public class AssignRoleClaimRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The role claim assignment information.
    /// </summary>
    [Required]
    [Body]
    public virtual AssignRoleClaim AssignRoleClaim { get; set; } = new();
}