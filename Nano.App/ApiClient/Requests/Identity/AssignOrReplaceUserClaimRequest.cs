using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class AssignOrReplaceUserClaimRequest : AssignOrReplaceUserClaimRequest<Guid>;

/// <summary>
/// Request to assign or replace a claim for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[PutAction(ActionRoutes.IDENTITY_CLAIMS_ASSIGN_OR_REPLACE)]
public class AssignOrReplaceUserClaimRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The claim assignment or replacement information.
    /// </summary>
    [Required]
    [Body]
    public virtual AssignOrReplaceUserClaim AssignOrReplaceUserClaim { get; set; } = new();
}