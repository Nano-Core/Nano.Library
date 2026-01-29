using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class AssignOrReplaceRoleClaimRequest : AssignOrReplaceRoleClaimRequest<Guid>;

/// <summary>
/// Request to assign or replace a claim for a role.
/// </summary>
/// <typeparam name="TIdentity">Type of the role identifier.</typeparam>
[PutAction(ActionRoutes.IDENTITY_ROLES_CLAIMS_ASSIGN_OR_REPLACE)]
public class AssignOrReplaceRoleClaimRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The role claim assignment or replacement information.
    /// </summary>
    [Required]
    [Body]
    public virtual AssignOrReplaceRoleClaim<TIdentity> AssignOrReplaceRoleClaim { get; set; } = new();
}