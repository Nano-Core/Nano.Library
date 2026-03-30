using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ReplaceRoleClaimRequest : ReplaceRoleClaimRequest<Guid>;

/// <summary>
/// Represents a request to replace a role's claim.
/// </summary>
/// <typeparam name="TIdentity">The type of the role identifier.</typeparam>
[PutAction(ActionRoutes.IDENTITY_ROLES_CLAIMS_REPLACE)]
public class ReplaceRoleClaimRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the role claim replacement information.
    /// </summary>
    [Required]
    [Body]
    public virtual ReplaceRoleClaim ReplaceRoleClaim { get; set; } = new();
}